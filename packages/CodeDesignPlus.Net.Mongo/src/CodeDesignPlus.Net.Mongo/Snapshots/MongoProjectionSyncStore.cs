using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.Mongo.Snapshots;

/// <summary>
/// Guarda en la base del micro, colección <c>ProjectionSyncState</c>, si cada copia local ya recibió la foto completa de
/// su fuente; y en <c>ProjectionSnapshotProgress</c> y <c>ProjectionSnapshotReceipt</c>, cuántas fotos de cada petición
/// aplicó.
/// </summary>
/// <remarks>
/// Un documento de estado por foto (aplicación dueña y evento de foto), con identificador derivado de ambos
/// (<see cref="ProjectionSyncState.IdOf"/>): dos réplicas que lo escriben a la vez escriben el mismo documento con un
/// upsert, sin carrera de creación. No es de ninguna copropiedad, así que la purga de una copropiedad no lo toca. Para
/// volver a pedir una foto completa basta con borrar el documento de esa foto y reiniciar el micro.
/// <para>
/// El progreso es un documento por petición (su identificador es el de la petición). Cada foto aplicada deja antes un
/// recibo con identificador derivado de la petición y del mensaje, así que una reentrega choca con la clave primaria y no
/// suma dos veces; y el cierre y la suma se escriben sobre el mismo documento con una operación atómica que devuelve el
/// resultado, así que quien escribe segundo ve lo que escribió el otro (pendings/485). Si el proceso cae entre el recibo
/// y la suma, la petición se queda corta: queda incompleta y se vuelve a pedir, nunca se da por buena de más.
/// </para>
/// </remarks>
internal sealed class MongoProjectionSyncStore(IMongoClient client, IOptions<MongoOptions> options) : IProjectionSyncStore
{
    /// <summary>
    /// El nombre de la colección del estado de cada foto.
    /// </summary>
    internal const string CollectionName = "ProjectionSyncState";

    /// <summary>
    /// El nombre de la colección del progreso de cada petición.
    /// </summary>
    internal const string ProgressCollectionName = "ProjectionSnapshotProgress";

    /// <summary>
    /// El nombre de la colección de los recibos de cada foto aplicada.
    /// </summary>
    internal const string ReceiptCollectionName = "ProjectionSnapshotReceipt";

    /// <summary>
    /// Cuántas veces se repite un upsert que choca con otro que creó el mismo documento a la vez.
    /// </summary>
    private const int UpsertAttempts = 3;

    private IMongoDatabase Database => client.GetDatabase(options.Value.Database);

    private IMongoCollection<ProjectionSyncDocument> Collection => this.Database.GetCollection<ProjectionSyncDocument>(CollectionName);

    private IMongoCollection<ProjectionSnapshotProgressDocument> Progress => this.Database.GetCollection<ProjectionSnapshotProgressDocument>(ProgressCollectionName);

    private IMongoCollection<ProjectionSnapshotReceiptDocument> Receipts => this.Database.GetCollection<ProjectionSnapshotReceiptDocument>(ReceiptCollectionName);

    /// <inheritdoc/>
    public async Task<ProjectionSyncState> FindAsync(SnapshotKey key, CancellationToken cancellationToken)
    {
        var id = ProjectionSyncState.IdOf(key);

        var document = await this.Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            return null;
        }

        return new ProjectionSyncState(document.Source, document.Aggregate, document.Snapshot, document.LastRequestId, document.RequestedAt, document.SyncedAt, document.Count)
        {
            Applied = document.Applied,
            IncompleteRequestId = document.IncompleteRequestId,
            IncompleteAt = document.IncompleteAt
        };
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
    public async Task MarkSyncedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSyncDocument>.Update
            .Set(x => x.Source, key.Source)
            .Set(x => x.Aggregate, key.Aggregate)
            .Set(x => x.Snapshot, key.Snapshot)
            .Set(x => x.SyncedRequestId, requestId)
            .Set(x => x.SyncedAt, SystemClock.Instance.GetCurrentInstant())
            .Set(x => x.Count, count)
            .Set(x => x.Applied, count)
            .Unset(x => x.IncompleteRequestId)
            .Unset(x => x.IncompleteAt);

        await this.UpsertAsync(key, update, cancellationToken).ConfigureAwait(false);

        // Sincronizada, la fuente no se vuelve a pedir: el progreso y los recibos de sus peticiones ya no sirven. Lo que
        // llegue después de otra petición en vuelo (otra réplica) deja unos pocos documentos que nadie lee.
        await this.Progress.DeleteManyAsync(x => x.Source == key.Source && x.Snapshot == key.Snapshot, cancellationToken).ConfigureAwait(false);
        await this.Receipts.DeleteManyAsync(x => x.Source == key.Source && x.Snapshot == key.Snapshot, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task MarkIncompleteAsync(SnapshotKey key, Guid requestId, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSyncDocument>.Update
            .Set(x => x.Source, key.Source)
            .Set(x => x.Aggregate, key.Aggregate)
            .Set(x => x.Snapshot, key.Snapshot)
            .Set(x => x.IncompleteRequestId, requestId)
            .Set(x => x.IncompleteAt, SystemClock.Instance.GetCurrentInstant());

        return this.UpsertAsync(key, update, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SnapshotProgress> RecordAppliedAsync(SnapshotKey key, Guid requestId, Guid eventId, CancellationToken cancellationToken)
    {
        var receipt = new ProjectionSnapshotReceiptDocument
        {
            Id = ProjectionSyncState.ReceiptIdOf(requestId, eventId),
            RequestId = requestId,
            EventId = eventId,
            Source = key.Source,
            Snapshot = key.Snapshot,
            AppliedAt = SystemClock.Instance.GetCurrentInstant()
        };

        try
        {
            await this.Receipts.InsertOneAsync(receipt, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            return null;
        }

        var update = Builders<ProjectionSnapshotProgressDocument>.Update.Inc(x => x.Applied, 1);

        return await this.UpsertProgressAsync(key, requestId, update, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<SnapshotProgress> RecordCompletedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSnapshotProgressDocument>.Update
            .Set(x => x.Expected, count)
            .Set(x => x.CompletedAt, SystemClock.Instance.GetCurrentInstant());

        return this.UpsertProgressAsync(key, requestId, update, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SnapshotProgress> FindProgressAsync(Guid requestId, CancellationToken cancellationToken)
    {
        var document = await this.Progress.Find(x => x.Id == requestId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            return null;
        }

        return new SnapshotProgress(document.Id, document.Applied, document.Expected);
    }

    private Task UpsertAsync(SnapshotKey key, UpdateDefinition<ProjectionSyncDocument> update, CancellationToken cancellationToken)
    {
        var id = ProjectionSyncState.IdOf(key);

        return this.Collection.UpdateOneAsync(x => x.Id == id, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
    }

    /// <summary>
    /// Aplica el cambio al progreso de la petición, creándolo si no existe, y devuelve cómo quedó.
    /// </summary>
    /// <remarks>
    /// Dos upserts que crean el mismo documento a la vez pueden chocar contra la clave primaria; el servidor no siempre lo
    /// reintenta (Azure DocumentDB no lo garantiza), así que se repite aquí: la segunda vez el documento ya existe.
    /// </remarks>
    private async Task<SnapshotProgress> UpsertProgressAsync(SnapshotKey key, Guid requestId, UpdateDefinition<ProjectionSnapshotProgressDocument> change, CancellationToken cancellationToken)
    {
        var update = Builders<ProjectionSnapshotProgressDocument>.Update.Combine(
            change,
            Builders<ProjectionSnapshotProgressDocument>.Update.SetOnInsert(x => x.Source, key.Source),
            Builders<ProjectionSnapshotProgressDocument>.Update.SetOnInsert(x => x.Aggregate, key.Aggregate),
            Builders<ProjectionSnapshotProgressDocument>.Update.SetOnInsert(x => x.Snapshot, key.Snapshot));

        var findOptions = new FindOneAndUpdateOptions<ProjectionSnapshotProgressDocument> { IsUpsert = true, ReturnDocument = ReturnDocument.After };

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var document = await this.Progress.FindOneAndUpdateAsync<ProjectionSnapshotProgressDocument>(x => x.Id == requestId, update, findOptions, cancellationToken).ConfigureAwait(false);

                return new SnapshotProgress(document.Id, document.Applied, document.Expected);
            }
            catch (MongoCommandException exception) when (exception.Code == 11000 && attempt < UpsertAttempts)
            {
                // Otro proceso creó el progreso entre la búsqueda y la inserción: ahora existe y el cambio se aplica.
            }
        }
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

    /// <summary>
    /// Cuántas aplicó la copia con éxito; nulo en una marca anterior a que se contaran (pendings/485).
    /// </summary>
    public long? Applied { get; set; }

    /// <summary>
    /// La última petición cerrada sin que la copia hubiera aplicado todas sus fotos.
    /// </summary>
    public Guid? IncompleteRequestId { get; set; }

    /// <summary>
    /// Cuándo se cerró esa petición.
    /// </summary>
    public Instant? IncompleteAt { get; set; }
}

/// <summary>
/// El progreso de una petición de foto en la copia.
/// </summary>
internal sealed class ProjectionSnapshotProgressDocument
{
    /// <summary>
    /// El identificador de la petición.
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
    /// Cuántas fotos aplicó la copia con éxito.
    /// </summary>
    public long Applied { get; set; }

    /// <summary>
    /// Cuántas anunció el dueño al cerrar, o nulo si el cierre no ha llegado.
    /// </summary>
    public long? Expected { get; set; }

    /// <summary>
    /// Cuándo llegó el cierre.
    /// </summary>
    public Instant? CompletedAt { get; set; }
}

/// <summary>
/// El recibo de una foto aplicada: existe para que una reentrega no cuente dos veces.
/// </summary>
internal sealed class ProjectionSnapshotReceiptDocument
{
    /// <summary>
    /// Derivado de la petición y del mensaje.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// La petición que contesta la foto.
    /// </summary>
    public Guid RequestId { get; set; }

    /// <summary>
    /// El identificador del mensaje de la foto.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// La aplicación dueña.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// El nombre del evento de foto.
    /// </summary>
    public string Snapshot { get; set; }

    /// <summary>
    /// Cuándo se aplicó.
    /// </summary>
    public Instant AppliedAt { get; set; }
}
