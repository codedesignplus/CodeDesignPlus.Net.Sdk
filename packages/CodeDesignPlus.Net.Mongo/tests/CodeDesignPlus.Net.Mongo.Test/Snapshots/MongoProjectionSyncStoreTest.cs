using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Snapshots;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Test.Snapshots;

/// <summary>
/// La copia recuerda en su base si ya recibió la foto completa de cada fuente, y lo recuerda entre arranques
/// (pendings/380). Solo la da por sincronizada si aplicó todas las fotos que anunció el dueño, cuenta cada foto una vez
/// y, lleguen a la vez las fotos y el cierre, alguno de los dos ve la cuenta completa (pendings/485).
/// </summary>
[Collection(MongoCollectionFixture.Collection)]
public class MongoProjectionSyncStoreTest
{
    private readonly MongoProjectionSyncStore store;
    private readonly IMongoDatabase database;

    public MongoProjectionSyncStoreTest(MongoCollectionFixture fixture)
    {
        MongoSerializerRegistration.RegisterSerializers();

        var options = OptionsUtil.GetOptions(fixture.Container.Port);

        var client = new MongoClient(options.ConnectionString);

        this.database = client.GetDatabase(options.Database);
        this.store = new MongoProjectionSyncStore(client, Microsoft.Extensions.Options.Options.Create(options));
    }

    [Fact]
    public async Task FindAsync_NeverRequested_ReturnsNull()
    {
        // Act
        var state = await this.store.FindAsync(Key("Never"), CancellationToken.None);

        // Assert
        Assert.Null(state);
    }

    [Fact]
    public async Task MarkRequestedAsync_NewSource_IsRequestedButNotSynced()
    {
        // Arrange
        var key = Key("Requested");
        var requestId = Guid.NewGuid();

        // Act
        await this.store.MarkRequestedAsync(key, requestId, CancellationToken.None);

        // Assert
        var state = await this.store.FindAsync(key, CancellationToken.None);

        Assert.Equal((requestId, false), (state!.LastRequestId, state.IsSynced));
    }

    [Fact]
    public async Task MarkSyncedAsync_AfterTheRequest_KeepsTheRequestAndIsSynced()
    {
        // Arrange
        var key = Key("Synced");
        var requestId = Guid.NewGuid();

        await this.store.MarkRequestedAsync(key, requestId, CancellationToken.None);

        // Act
        await this.store.MarkSyncedAsync(key, requestId, 3, CancellationToken.None);

        // Assert
        var state = await this.store.FindAsync(key, CancellationToken.None);

        Assert.Equal((requestId, true, 3L, 3L), (state!.LastRequestId, state.IsSynced, state.Count, state.Applied));
    }

    /// <summary>
    /// Una marca guardada antes de que se contaran las fotos aplicadas trae fecha y cuenta del dueño, pero no
    /// <c>Applied</c>: se lee sin error y no cuenta como sincronizada.
    /// </summary>
    [Fact]
    public async Task FindAsync_SyncedBeforeAppliedWasCounted_IsNotSynced()
    {
        // Arrange
        var key = Key("Legacy");

        await this.store.MarkSyncedAsync(key, Guid.NewGuid(), 28, CancellationToken.None);

        await this.database.GetCollection<BsonDocument>(MongoProjectionSyncStore.CollectionName).UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ProjectionSyncState.IdOf(key)), Builders<BsonDocument>.Update.Unset("Applied").Unset("IncompleteRequestId").Unset("IncompleteAt"));

        // Act
        var state = await this.store.FindAsync(key, CancellationToken.None);

        // Assert
        Assert.Equal((true, false, 28L), (state!.SyncedAt.HasValue, state.IsSynced, state.Count));
    }

    [Fact]
    public async Task MarkIncompleteAsync_NeverSynced_IsIncomplete()
    {
        // Arrange
        var key = Key("Incomplete");
        var requestId = Guid.NewGuid();

        // Act
        await this.store.MarkIncompleteAsync(key, requestId, CancellationToken.None);

        // Assert
        var state = await this.store.FindAsync(key, CancellationToken.None);

        Assert.Equal((false, true, requestId), (state!.IsSynced, state.IsIncomplete, state.IncompleteRequestId));
    }

    [Fact]
    public async Task RecordAppliedAsync_SameMessageTwice_CountsOnce()
    {
        // Arrange
        var key = Key("Redelivered");
        var requestId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        await this.store.RecordAppliedAsync(key, requestId, eventId, CancellationToken.None);

        // Act
        var again = await this.store.RecordAppliedAsync(key, requestId, eventId, CancellationToken.None);

        // Assert
        var progress = await this.store.FindProgressAsync(requestId, CancellationToken.None);

        Assert.Equal((true, 1L), (again is null, progress!.Applied));
    }

    [Fact]
    public async Task RecordCompletedAsync_BeforeTheSnapshots_IsCompletedByTheLastOne()
    {
        // Arrange
        var key = Key("CompletedFirst");
        var requestId = Guid.NewGuid();

        var completed = await this.store.RecordCompletedAsync(key, requestId, 2, CancellationToken.None);
        var first = await this.store.RecordAppliedAsync(key, requestId, Guid.NewGuid(), CancellationToken.None);

        // Act
        var last = await this.store.RecordAppliedAsync(key, requestId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal((false, false, true, 2L), (completed.IsComplete, first!.IsComplete, last!.IsComplete, last.Applied));
    }

    [Fact]
    public async Task MarkSyncedAsync_WithProgress_ClearsTheProgressOfThatSnapshot()
    {
        // Arrange
        var key = Key("Cleared");
        var requestId = Guid.NewGuid();

        await this.store.RecordAppliedAsync(key, requestId, Guid.NewGuid(), CancellationToken.None);
        await this.store.RecordCompletedAsync(key, requestId, 1, CancellationToken.None);

        // Act
        await this.store.MarkSyncedAsync(key, requestId, 1, CancellationToken.None);

        // Assert
        var progress = await this.store.FindProgressAsync(requestId, CancellationToken.None);
        var receipts = await this.database.GetCollection<BsonDocument>(MongoProjectionSyncStore.ReceiptCollectionName).CountDocumentsAsync(Builders<BsonDocument>.Filter.Eq("RequestId", requestId));

        Assert.Equal((true, 0L), (progress is null, receipts));
    }

    /// <summary>
    /// Las fotos y el cierre se escriben a la vez desde varios hilos, como desde varias réplicas: la cuenta final es exacta
    /// y al menos una de las escrituras ve la petición completa, que es la que la marca.
    /// </summary>
    [Fact]
    public async Task RecordAsync_SnapshotsAndCompletionConcurrently_SomeWriterSeesItComplete()
    {
        // Arrange
        var key = Key("Concurrent");
        var requestId = Guid.NewGuid();

        var applied = Enumerable.Range(0, 30).Select(_ => Task.Run(() => this.store.RecordAppliedAsync(key, requestId, Guid.NewGuid(), CancellationToken.None)));
        var completed = Task.Run<SnapshotProgress?>(async () => await this.store.RecordCompletedAsync(key, requestId, 30, CancellationToken.None));

        // Act
        var results = await Task.WhenAll(applied.Append(completed));

        // Assert
        var progress = await this.store.FindProgressAsync(requestId, CancellationToken.None);

        Assert.Equal((30L, 30L, true), (progress!.Applied, progress.Expected, results.Any(x => x!.IsComplete)));
    }

    private static SnapshotKey Key(string prefix) => new("ms-assemblies", "AssemblyAggregate", $"{prefix}{Guid.NewGuid():N}SnapshotDomainEvent");
}
