using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Snapshots;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Test.Snapshots;

/// <summary>
/// La copia recuerda en su base si ya recibió la foto completa de cada fuente, y lo recuerda entre arranques
/// (pendings/380).
/// </summary>
[Collection(MongoCollectionFixture.Collection)]
public class MongoProjectionSyncStoreTest
{
    private readonly MongoProjectionSyncStore store;

    public MongoProjectionSyncStoreTest(MongoCollectionFixture fixture)
    {
        MongoSerializerRegistration.RegisterSerializers();

        var options = OptionsUtil.GetOptions(fixture.Container.Port);

        this.store = new MongoProjectionSyncStore(new MongoClient(options.ConnectionString), Microsoft.Extensions.Options.Options.Create(options));
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

        Assert.Equal((requestId, true, 3L), (state!.LastRequestId, state.IsSynced, state.Count));
    }

    private static SnapshotKey Key(string prefix) => new("ms-assemblies", "AssemblyAggregate", $"{prefix}{Guid.NewGuid():N}SnapshotDomainEvent");
}
