using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.Mongo.Snapshots;

/// <summary>
/// Guarda en la base del micro, colección <c>ProjectionSyncState</c>, si cada copia local ya recibió la foto completa de
/// su fuente.
/// </summary>
/// <remarks>
/// Un documento por fuente y agregado, con identificador derivado de ambos (<see cref="ProjectionSyncState.IdOf"/>):
/// dos réplicas que lo escriben a la vez escriben el mismo documento con un upsert, sin carrera de creación. No es de
/// ninguna copropiedad, así que la purga de una copropiedad no lo toca. Para volver a pedir una foto completa basta con
/// borrar el documento de esa fuente y reiniciar el micro.
/// </remarks>
internal sealed class MongoProjectionSyncStore(IMongoClient client, IOptions<MongoOptions> options) : IProjectionSyncStore
{
    /// <summary>
    /// El nombre de la colección.
    /// </summary>
    internal const string CollectionName = "ProjectionSyncState";

    private IMongoCollection<ProjectionSyncDocument> Collection => client.GetDatabase(options.Value.Database).GetCollection<ProjectionSyncDocument>(CollectionName);

    /// <inheritdoc/>
    public async Task<ProjectionSyncState> FindAsync(string source, string aggregate, CancellationToken cancellationToken)
    {
        var document = await this.Collection.Find(x => x.Id == ProjectionSyncState.IdOf(source, aggregate)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            return null;
        }

        return new ProjectionSyncState(document.Source, document.Aggregate, document.LastRequestId, document.RequestedAt, document.SyncedAt, document.Count);
    }

    /// <inheritdoc/>
    public Task MarkRequestedAsync(string source, string aggregate, Guid requestId, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSyncDocument>.Update
            .Set(x => x.Source, source)
            .Set(x => x.Aggregate, aggregate)
            .Set(x => x.LastRequestId, requestId)
            .Set(x => x.RequestedAt, SystemClock.Instance.GetCurrentInstant());

        return this.UpsertAsync(source, aggregate, update, cancellationToken);
    }

    /// <inheritdoc/>
    public Task MarkSyncedAsync(string source, string aggregate, Guid requestId, long count, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSyncDocument>.Update
            .Set(x => x.Source, source)
            .Set(x => x.Aggregate, aggregate)
            .Set(x => x.SyncedRequestId, requestId)
            .Set(x => x.SyncedAt, SystemClock.Instance.GetCurrentInstant())
            .Set(x => x.Count, count);

        return this.UpsertAsync(source, aggregate, update, cancellationToken);
    }

    private Task UpsertAsync(string source, string aggregate, UpdateDefinition<ProjectionSyncDocument> update, CancellationToken cancellationToken)
    {
        var id = ProjectionSyncState.IdOf(source, aggregate);

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
