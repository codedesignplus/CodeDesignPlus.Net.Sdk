using CodeDesignPlus.Net.Mongo.Abstractions.Exceptions;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Test.Repository;

[Collection(MongoCollectionFixture.Collection)]
public class VersionedRepositoryTest
{
    private readonly IOptions<MongoOptions> options;
    private readonly IMongoCollection<CounterAggregate> collection;
    private readonly ClientRepository repository;

    public VersionedRepositoryTest(MongoCollectionFixture fixture)
    {
        try
        {
            BsonSerializer.TryRegisterSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch { }

        var mongoOptions = OptionsUtil.GetOptions(fixture.Container.Port);

        mongoOptions.ConcurrencyRetryDelayMilliseconds = 5;

        this.options = Microsoft.Extensions.Options.Options.Create(mongoOptions);

        var client = new MongoClient(mongoOptions.ConnectionString);

        this.collection = client.GetDatabase(mongoOptions.Database).GetCollection<CounterAggregate>(nameof(CounterAggregate));

        var services = new ServiceCollection();

        services.AddSingleton<IMongoClient>(client);

        this.repository = new ClientRepository(services.BuildServiceProvider(), this.options, Mock.Of<ILogger<ClientRepository>>());
    }

    [Fact]
    public async Task CreateAsync_NewAggregate_StoresVersionOne()
    {
        // Arrange
        var counter = CounterAggregate.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        await this.repository.CreateAsync(counter, CancellationToken.None);

        // Assert
        var stored = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        Assert.Equal(1, stored.Version);
        Assert.Equal(1, counter.Version);
    }

    [Fact]
    public async Task UpdateAsync_StaleVersion_ThrowsConflictAndKeepsTheStoredDocument()
    {
        // Arrange
        var counter = CounterAggregate.Create(Guid.NewGuid(), Guid.NewGuid());

        await this.repository.CreateAsync(counter, CancellationToken.None);

        var first = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();
        var second = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        first.Increment();
        await this.repository.UpdateAsync(first, CancellationToken.None);

        second.Increment();
        second.Increment();

        // Act
        var exception = await Record.ExceptionAsync(() => this.repository.UpdateAsync(second, CancellationToken.None));

        // Assert
        var stored = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        Assert.IsType<ConcurrencyConflictException>(exception);
        Assert.Equal(1, stored.Value);
        Assert.Equal(2, stored.Version);
        Assert.Equal(1, second.Version);
    }

    [Fact]
    public async Task UpdateAsync_FreshVersion_RaisesTheVersion()
    {
        // Arrange
        var counter = CounterAggregate.Create(Guid.NewGuid(), Guid.NewGuid());

        await this.repository.CreateAsync(counter, CancellationToken.None);

        // Act
        counter.Increment();
        await this.repository.UpdateAsync(counter, CancellationToken.None);
        counter.Increment();
        await this.repository.UpdateAsync(counter, CancellationToken.None);

        // Assert
        var stored = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        Assert.Equal(2, stored.Value);
        Assert.Equal(3, stored.Version);
    }

    [Fact]
    public async Task UpsertAsync_NewAggregateOverAnExistingId_ThrowsConflictInsteadOfReplacing()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = CounterAggregate.Create(id, Guid.NewGuid());

        existing.Increment();
        await this.repository.CreateAsync(existing, CancellationToken.None);

        var newcomer = CounterAggregate.Create(id, Guid.NewGuid());

        // Act
        var exception = await Record.ExceptionAsync(() => this.repository.UpsertAsync(newcomer, CancellationToken.None));

        // Assert
        var stored = await this.collection.Find(x => x.Id == id).FirstAsync();

        Assert.IsType<ConcurrencyConflictException>(exception);
        Assert.Equal(1, stored.Value);
        Assert.Equal(0, newcomer.Version);
    }

    [Fact]
    public async Task FindAsync_DocumentWithoutVersion_ReadsAsZeroAndCanBeUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();

        await this.InsertWithoutVersionAsync(id, 7);

        var counter = await this.collection.Find(x => x.Id == id).FirstAsync();

        // Act
        counter.Increment();
        await this.repository.UpdateAsync(counter, CancellationToken.None);

        // Assert
        var stored = await this.collection.Find(x => x.Id == id).FirstAsync();

        Assert.Equal(8, stored.Value);
        Assert.Equal(1, stored.Version);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_DocumentWithoutVersion_IsUpdatedInsteadOfConflictingForever()
    {
        // Arrange: a document saved before the aggregate was versioned reads as version 0, like a new one.
        var id = Guid.NewGuid();

        await this.InsertWithoutVersionAsync(id, 7);

        // Act
        var written = await this.repository.UpdateWithRetryAsync(
            () => this.collection.Find(x => x.Id == id).FirstOrDefaultAsync(),
            aggregate =>
            {
                aggregate.Increment();

                return true;
            },
            maxAttempts: 3,
            CancellationToken.None);

        // Assert
        var stored = await this.collection.Find(x => x.Id == id).FirstAsync();

        Assert.Equal((true, 8, 1L), (written, stored.Value, stored.Version));
    }

    [Fact]
    public async Task UpdateWithRetryAsync_FiftyConcurrentIncrementsOnADocumentWithoutVersion_KeepsAllFifty()
    {
        // Arrange
        var id = Guid.NewGuid();

        await this.InsertWithoutVersionAsync(id, 0);

        // Act
        await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => Task.Run(() => this.repository.UpdateWithRetryAsync(
            () => this.collection.Find(x => x.Id == id).FirstOrDefaultAsync(),
            aggregate =>
            {
                aggregate.Increment();

                return true;
            },
            maxAttempts: 100,
            CancellationToken.None))));

        // Assert
        var stored = await this.collection.Find(x => x.Id == id).FirstAsync();

        Assert.Equal(50, stored.Value);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_FiftyConcurrentIncrements_KeepsAllFifty()
    {
        // Arrange
        var counter = CounterAggregate.Create(Guid.NewGuid(), Guid.NewGuid());

        await this.repository.CreateAsync(counter, CancellationToken.None);

        // Act
        var writers = Enumerable.Range(0, 50).Select(_ => Task.Run(() => this.repository.UpdateWithRetryAsync(
            () => this.collection.Find(x => x.Id == counter.Id).FirstOrDefaultAsync(),
            aggregate =>
            {
                aggregate.Increment();

                return true;
            },
            maxAttempts: 100,
            CancellationToken.None)));

        await Task.WhenAll(writers);

        // Assert
        var stored = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        Assert.Equal(50, stored.Value);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_ConcurrentCreatesOfTheSameId_CreateOnceAndCountEveryWriter()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenant = Guid.NewGuid();

        // Act
        var writers = Enumerable.Range(0, 20).Select(_ => Task.Run(() => this.repository.UpdateWithRetryAsync(
            async () => await this.collection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? CounterAggregate.Create(id, tenant),
            aggregate =>
            {
                aggregate.Increment();

                return true;
            },
            maxAttempts: 100,
            CancellationToken.None)));

        await Task.WhenAll(writers);

        // Assert
        var stored = await this.collection.Find(x => x.Id == id).ToListAsync();

        Assert.Equal(20, Assert.Single(stored).Value);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_MutateDeclines_DoesNotWrite()
    {
        // Arrange
        var counter = CounterAggregate.Create(Guid.NewGuid(), Guid.NewGuid());
        var counted = Guid.NewGuid();

        counter.Count(counted);
        await this.repository.CreateAsync(counter, CancellationToken.None);

        // Act
        var written = await this.repository.UpdateWithRetryAsync(
            () => this.collection.Find(x => x.Id == counter.Id).FirstOrDefaultAsync(),
            aggregate => aggregate.Count(counted),
            CancellationToken.None);

        // Assert
        var stored = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        Assert.False(written);
        Assert.Equal(1, stored.Version);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_LoadReturnsNull_DoesNotWrite()
    {
        // Act
        var written = await this.repository.UpdateWithRetryAsync<CounterAggregate>(() => Task.FromResult<CounterAggregate>(null!), _ => true, CancellationToken.None);

        // Assert
        Assert.False(written);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_EveryAttemptConflicts_ThrowsConflict()
    {
        // Arrange
        var counter = CounterAggregate.Create(Guid.NewGuid(), Guid.NewGuid());

        await this.repository.CreateAsync(counter, CancellationToken.None);

        var stale = await this.collection.Find(x => x.Id == counter.Id).FirstAsync();

        counter.Increment();
        await this.repository.UpdateAsync(counter, CancellationToken.None);

        // Act
        var exception = await Record.ExceptionAsync(() => this.repository.UpdateWithRetryAsync(() => Task.FromResult(stale), aggregate => true, maxAttempts: 3, CancellationToken.None));

        // Assert
        Assert.IsType<ConcurrencyConflictException>(exception);
    }

    [Fact]
    public async Task UpdateWithRetryAsync_ZeroAttempts_ThrowsMongoException()
    {
        // Act
        var exception = await Record.ExceptionAsync(() => this.repository.UpdateWithRetryAsync<CounterAggregate>(() => Task.FromResult<CounterAggregate>(null!), _ => true, maxAttempts: 0, CancellationToken.None));

        // Assert
        Assert.IsType<Mongo.Exceptions.MongoException>(exception);
    }

    [Fact]
    public async Task CreateAsync_VersionWithoutSetter_ThrowsMongoException()
    {
        // Act
        var exception = await Record.ExceptionAsync(() => this.repository.CreateAsync(new UnversionableAggregate(Guid.NewGuid()), CancellationToken.None));

        // Assert
        Assert.IsType<Mongo.Exceptions.MongoException>(exception);
    }

    [Fact]
    public void MongoOptions_ZeroAttempts_IsInvalid()
    {
        // Arrange
        var mongoOptions = OptionsUtil.GetOptions(27017);

        mongoOptions.ConcurrencyMaxAttempts = 0;

        // Act
        var results = mongoOptions.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(mongoOptions));

        // Assert
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(MongoOptions.ConcurrencyMaxAttempts)));
    }

    private Task InsertWithoutVersionAsync(Guid id, int value)
    {
        var raw = this.collection.Database.GetCollection<BsonDocument>(nameof(CounterAggregate));

        return raw.InsertOneAsync(new BsonDocument
        {
            { "_id", new BsonBinaryData(id, GuidRepresentation.Standard) },
            { "Value", value },
            { "CountedIds", new BsonArray() },
            { "Tenant", new BsonBinaryData(Guid.NewGuid(), GuidRepresentation.Standard) },
            { "IsActive", true },
            { "IsDeleted", false },
            { "CreatedBy", new BsonBinaryData(Guid.Empty, GuidRepresentation.Standard) },
        });
    }
}
