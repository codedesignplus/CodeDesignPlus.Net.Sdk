using CodeDesignPlus.Net.Core.Abstractions.Outbox;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Outbox;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Test.Outbox;

/// <summary>
/// La bandeja de salida en la base del micro: una réplica reclama cada mensaje, lo que falla se reprograma, lo agotado
/// se queda y lo enviado se borra pasado su plazo (pendings/245).
/// </summary>
[Collection(MongoCollectionFixture.Collection)]
public class MongoOutboxStoreTest
{
    private static readonly Duration Lease = Duration.FromMinutes(2);

    private readonly MongoOutboxStore store;
    private readonly IMongoCollection<BsonDocument> collection;

    public MongoOutboxStoreTest(MongoCollectionFixture fixture)
    {
        MongoSerializerRegistration.RegisterSerializers();

        // Una base por prueba: el reclamo toma el pendiente más antiguo de toda la colección.
        var options = OptionsUtil.GetOptions(fixture.Container.Port);

        options.Database = $"outbox{Guid.NewGuid():N}";

        var client = new MongoClient(options.ConnectionString);

        this.store = new MongoOutboxStore(client, Microsoft.Extensions.Options.Options.Create(options));
        this.collection = client.GetDatabase(options.Database).GetCollection<BsonDocument>(MongoOutboxStore.CollectionName);
    }

    [Fact]
    public async Task LeaseNextAsync_Enqueued_ReturnsTheMessageAsItWasEnqueued()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        // Act
        var leased = await this.store.LeaseNextAsync("replica-a", Lease, CancellationToken.None);

        // Assert
        Assert.Equal((message.Id, message.Topic, message.EventName, message.AggregateId, message.Body, "value"), (leased.Id, leased.Topic, leased.EventName, leased.AggregateId, leased.Body, leased.Headers["traceparent"]));
    }

    [Fact]
    public async Task LeaseNextAsync_AlreadyLeased_ReturnsNothingToAnotherReplica()
    {
        // Arrange
        await this.store.AddAsync([NewMessage()], CancellationToken.None);

        await this.store.LeaseNextAsync("replica-a", Lease, CancellationToken.None);

        // Act
        var leased = await this.store.LeaseNextAsync("replica-b", Lease, CancellationToken.None);

        // Assert
        Assert.Null(leased);
    }

    [Fact]
    public async Task LeaseNextAsync_LeaseExpired_AnotherReplicaTakesIt()
    {
        // Arrange: la réplica que lo reclamó murió a mitad del envío.
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        await this.store.LeaseNextAsync("replica-a", Duration.FromMilliseconds(1), CancellationToken.None);

        await Task.Delay(50);

        // Act
        var leased = await this.store.LeaseNextAsync("replica-b", Lease, CancellationToken.None);

        // Assert
        Assert.Equal(message.Id, leased?.Id);
    }

    [Fact]
    public async Task LeaseNextAsync_ConcurrentReplicas_EachMessageLeasedOnce()
    {
        // Arrange
        var messages = Enumerable.Range(0, 20).Select(_ => NewMessage()).ToList();

        await this.store.AddAsync(messages, CancellationToken.None);

        // Act
        var leased = await Task.WhenAll(Enumerable.Range(0, 40).Select(i => this.store.LeaseNextAsync($"replica-{i}", Lease, CancellationToken.None)));

        // Assert
        Assert.Equal(messages.Select(x => x.Id).Order(), leased.Where(x => x is not null).Select(x => x.Id).Order());
    }

    [Fact]
    public async Task AddAsync_SameEventTwice_KeepsOne()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        // Act
        await this.store.AddAsync([message, NewMessage()], CancellationToken.None);

        // Assert
        Assert.Equal(2, await this.collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty));
    }

    [Fact]
    public async Task MarkSentAsync_Sent_IsNotLeasedAgain()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        await this.store.LeaseNextAsync("replica-a", Duration.FromMilliseconds(1), CancellationToken.None);

        // Act
        await this.store.MarkSentAsync(message.Id, CancellationToken.None);

        // Assert
        await Task.Delay(50);

        Assert.Null(await this.store.LeaseNextAsync("replica-b", Lease, CancellationToken.None));
    }

    [Fact]
    public async Task MarkSentAsync_Sent_StoresWhenAsADateForTheTtl()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        // Act
        await this.store.MarkSentAsync(message.Id, CancellationToken.None);

        // Assert
        var document = await this.collection.Find(new BsonDocument("_id", new BsonBinaryData(message.Id, GuidRepresentation.Standard))).SingleAsync();

        Assert.Equal(("Sent", BsonType.DateTime), (document["Status"].AsString, document["SentAt"].BsonType));
    }

    [Fact]
    public async Task MarkFailedAsync_WithNextAttempt_WaitsUntilThen()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        // Act
        await this.store.MarkFailedAsync(message.Id, "Service Bus is down.", SystemClock.Instance.GetCurrentInstant() + Duration.FromHours(1), CancellationToken.None);

        // Assert
        Assert.Null(await this.store.LeaseNextAsync("replica-a", Lease, CancellationToken.None));
    }

    [Fact]
    public async Task MarkFailedAsync_NextAttemptDue_LeasedWithTheAttemptCounted()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        // Act
        await this.store.MarkFailedAsync(message.Id, "Service Bus is down.", SystemClock.Instance.GetCurrentInstant(), CancellationToken.None);

        // Assert
        var leased = await this.store.LeaseNextAsync("replica-a", Lease, CancellationToken.None);

        Assert.Equal(1, leased?.Attempts);
    }

    [Fact]
    public async Task MarkFailedAsync_Exhausted_StaysFailedAndIsNotLeased()
    {
        // Arrange
        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        // Act
        await this.store.MarkFailedAsync(message.Id, "Service Bus is down.", null, CancellationToken.None);

        // Assert
        var document = await this.collection.Find(new BsonDocument("_id", new BsonBinaryData(message.Id, GuidRepresentation.Standard))).SingleAsync();
        var leased = await this.store.LeaseNextAsync("replica-a", Lease, CancellationToken.None);

        Assert.Equal(("Failed", "Service Bus is down.", true), (document["Status"].AsString, document["LastError"].AsString, leased is null));
    }

    [Fact]
    public async Task EnsureIndexesAsync_SevenDays_CreatesTheTtlOnSentAt()
    {
        // Act
        await this.store.EnsureIndexesAsync(Duration.FromDays(7), CancellationToken.None);

        // Assert
        Assert.Equal(7 * 24 * 3600, await this.ExpireAfterSecondsAsync());
    }

    [Fact]
    public async Task EnsureIndexesAsync_RetentionChanged_ReplacesTheTtl()
    {
        // Arrange
        await this.store.EnsureIndexesAsync(Duration.FromDays(7), CancellationToken.None);

        // Act
        await this.store.EnsureIndexesAsync(Duration.FromDays(3), CancellationToken.None);

        // Assert
        Assert.Equal(3 * 24 * 3600, await this.ExpireAfterSecondsAsync());
    }

    private async Task<long> ExpireAfterSecondsAsync()
    {
        var indexes = await (await this.collection.Indexes.ListAsync()).ToListAsync();

        var ttl = indexes.Single(x => x["name"] == MongoOutboxStore.SentRetentionIndexName);

        return ttl["expireAfterSeconds"].ToInt64();
    }

    private static OutboxMessage NewMessage()
    {
        return new OutboxMessage(Guid.NewGuid(), "codedesignplus.ms-test.v1.client.created", "ClientCreatedDomainEvent", Guid.NewGuid(), "{\"Name\":\"Code\"}", new Dictionary<string, string> { ["traceparent"] = "value" }, SystemClock.Instance.GetCurrentInstant(), 0);
    }
}
