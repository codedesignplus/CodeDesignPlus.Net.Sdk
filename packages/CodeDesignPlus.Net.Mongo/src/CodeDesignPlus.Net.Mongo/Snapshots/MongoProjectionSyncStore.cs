using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.Mongo.Snapshots;

/// <summary>
/// Guarda en la base del micro, colección <c>ProjectionSyncState</c>, si cada copia local ya recibió la foto completa de
/// su fuente.
/// </summary>
/// <remarks>
/// Un documento por foto (aplicación dueña y evento de foto), con identificador derivado de ambos (<see cref="ProjectionSyncState.IdOf"/>):
/// dos réplicas que lo escriben a la vez escriben el mismo documento con un upsert, sin carrera de creación. No es de
/// ninguna copropiedad, así que la purga de una copropiedad no lo toca. Para volver a pedir una foto completa basta con
/// borrar el documento de esa foto y reiniciar el micro.
/// </remarks>
internal sealed class MongoProjectionSyncStore(IMongoClient client, IOptions<MongoOptions> options) : IProjectionSyncStore
{
    /// <summary>
    /// El nombre de la colección.
    /// </summary>
    internal const string CollectionName = "ProjectionSyncState";

    private IMongoCollection<ProjectionSyncDocument> Collection => client.GetDatabase(options.Value.Database).GetCollection<ProjectionSyncDocument>(CollectionName);

    /// <inheritdoc/>
    public async Task<ProjectionSyncState> FindAsync(SnapshotKey key, CancellationToken cancellationToken)
    {
        var id = ProjectionSyncState.IdOf(key);

        var document = await this.Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            return null;
        }

        return new ProjectionSyncState(document.Source, document.Aggregate, document.Snapshot, document.LastRequestId, document.RequestedAt, document.SyncedAt, document.Count);
    }

    /// <inheritdoc/>
    public Task MarkRequestedAsync(SnapshotKey key, Guid requestId, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSyncDocument>.Update
            .Set(x => x.Source, key.Source)
            .Set(x => x.Aggregate, key.Aggregate)
            .Set(x => x.Snapshot, key.Snapshot)
            .Set(x => x.LastRequestId, requestId)
            .Set(x => x.RequestedAt, SystemClock.Instance.GetCurrentInstant());

        return this.UpsertAsync(key, update, cancellationToken);
    }

    /// <inheritdoc/>
    public Task MarkSyncedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSyncDocument>.Update
            .Set(x => x.Source, key.Source)
            .Set(x => x.Aggregate, key.Aggregate)
            .Set(x => x.Snapshot, key.Snapshot)
            .Set(x => x.SyncedRequestId, requestId)
            .Set(x => x.SyncedAt, SystemClock.Instance.GetCurrentInstant())
            .Set(x => x.Count, count);

        return this.UpsertAsync(key, update, cancellationToken);
    }

    private Task UpsertAsync(SnapshotKey key, UpdateDefinition<ProjectionSyncDocument> update, CancellationToken cancellationToken)
    {
        var id = ProjectionSyncState.IdOf(key);

        return this.Collection.UpdateOneAsync(x => x.Id == id, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
    }
}

/// <summary>
/// El documento de estado de sincronización de una fuente.
/// </summary>
internal sealed class ProjectionSyncDocument
{
    /// <summary>
    /// Derivado de la fuente y el agregado.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// La aplicación dueña.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// El nombre del agregado.
    /// </summary>
    public string Aggregate { get; set; }

    /// <summary>
    /// El nombre del evento de foto.
    /// </summary>
    public string Snapshot { get; set; }

    /// <summary>
    /// La última petición publicada.
    /// </summary>
    public Guid? LastRequestId { get; set; }

    /// <summary>
    /// Cuándo se publicó la última petición.
    /// </summary>
    public Instant? RequestedAt { get; set; }

    /// <summary>
    /// La petición cuya foto completa terminó.
    /// </summary>
    public Guid? SyncedRequestId { get; set; }

    /// <summary>
    /// Cuándo terminó la última foto completa.
    /// </summary>
    public Instant? SyncedAt { get; set; }

    /// <summary>
    /// Cuántas fotos trajo.
    /// </summary>
    public long Count { get; set; }
}
