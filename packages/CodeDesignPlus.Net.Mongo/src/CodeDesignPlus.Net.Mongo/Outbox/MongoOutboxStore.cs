using CodeDesignPlus.Net.Core.Abstractions.Outbox;

namespace CodeDesignPlus.Net.Mongo.Outbox;

/// <summary>
/// La bandeja de salida en la base del micro, colección <c>Outbox</c> (pendings/245).
/// </summary>
/// <remarks>
/// Un documento por evento, con el identificador del evento: encolarlo dos veces no lo duplica. Lo enviado se borra solo
/// pasado el plazo de retención (índice TTL sobre <see cref="OutboxDocument.SentAt"/>); lo fallido se queda hasta que
/// alguien lo mire. No es de ninguna copropiedad, así que la purga de una copropiedad no lo toca.
/// </remarks>
internal sealed class MongoOutboxStore(IMongoClient client, IOptions<MongoOptions> options) : IOutboxStore
{
    /// <summary>
    /// El nombre de la colección.
    /// </summary>
    internal const string CollectionName = "Outbox";

    /// <summary>
    /// El nombre del índice que borra lo enviado.
    /// </summary>
    internal const string SentRetentionIndexName = "SentAt_ttl";

    /// <summary>
    /// El código de error de Mongo cuando un índice ya existe con otras opciones.
    /// </summary>
    private const int IndexOptionsConflict = 85;

    private IMongoCollection<OutboxDocument> Collection => client.GetDatabase(options.Value.Database).GetCollection<OutboxDocument>(CollectionName);

    /// <inheritdoc/>
    public async Task EnsureIndexesAsync(Duration sentRetention, CancellationToken cancellationToken)
    {
        var pending = new CreateIndexModel<OutboxDocument>(Builders<OutboxDocument>.IndexKeys
            .Ascending(x => x.Status)
            .Ascending(x => x.NextAttemptAt)
            .Ascending(x => x.CreatedAt), new CreateIndexOptions { Name = "Status_NextAttemptAt_CreatedAt" });

        await this.Collection.Indexes.CreateOneAsync(pending, cancellationToken: cancellationToken).ConfigureAwait(false);

        var retention = new CreateIndexModel<OutboxDocument>(Builders<OutboxDocument>.IndexKeys.Ascending(x => x.SentAt), new CreateIndexOptions
        {
            Name = SentRetentionIndexName,
            ExpireAfter = sentRetention.ToTimeSpan()
        });

        try
        {
            await this.Collection.Indexes.CreateOneAsync(retention, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (MongoCommandException exception) when (exception.Code == IndexOptionsConflict)
        {
            // Cambió el plazo de retención: el índice se rehace con el nuevo.
            await this.Collection.Indexes.DropOneAsync(SentRetentionIndexName, cancellationToken).ConfigureAwait(false);

            await this.Collection.Indexes.CreateOneAsync(retention, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task AddAsync(IReadOnlyList<OutboxMessage> messages, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(messages);

        if (messages.Count == 0)
        {
            return;
        }

        var documents = messages.Select(OutboxDocument.From).ToList();

        try
        {
            await this.Collection.InsertManyAsync(documents, new InsertManyOptions { IsOrdered = false }, cancellationToken).ConfigureAwait(false);
        }
        catch (MongoBulkWriteException<OutboxDocument> exception) when (exception.WriteErrors.All(x => x.Category == ServerErrorCategory.DuplicateKey))
        {
            // El evento ya estaba encolado: se publicó dos veces el mismo y basta con una.
        }
    }

    /// <inheritdoc/>
    public async Task<OutboxMessage> LeaseNextAsync(string owner, Duration lease, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(owner);

        var now = SystemClock.Instance.GetCurrentInstant();

        var filter = Builders<OutboxDocument>.Filter.And(
            Builders<OutboxDocument>.Filter.Eq(x => x.Status, OutboxStatus.Pending),
            Builders<OutboxDocument>.Filter.Lte(x => x.NextAttemptAt, now),
            Builders<OutboxDocument>.Filter.Or(
                Builders<OutboxDocument>.Filter.Eq(x => x.LockedUntil, null),
                Builders<OutboxDocument>.Filter.Lt(x => x.LockedUntil, now)));

        var update = Builders<OutboxDocument>.Update
            .Set(x => x.LockedUntil, now + lease)
            .Set(x => x.LockedBy, owner);

        var findOptions = new FindOneAndUpdateOptions<OutboxDocument>
        {
            Sort = Builders<OutboxDocument>.Sort.Ascending(x => x.CreatedAt),
            ReturnDocument = ReturnDocument.After
        };

        var document = await this.Collection.FindOneAndUpdateAsync(filter, update, findOptions, cancellationToken).ConfigureAwait(false);

        return document?.ToMessage();
    }

    /// <inheritdoc/>
    public Task MarkSentAsync(Guid id, CancellationToken cancellationToken)
    {
        var update = Builders<OutboxDocument>.Update
            .Set(x => x.Status, OutboxStatus.Sent)
            .Set(x => x.SentAt, SystemClock.Instance.GetCurrentInstant())
            .Set(x => x.LockedUntil, null)
            .Set(x => x.LockedBy, null);

        return this.Collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public Task MarkFailedAsync(Guid id, string error, Instant? nextAttemptAt, CancellationToken cancellationToken)
    {
        var update = Builders<OutboxDocument>.Update
            .Inc(x => x.Attempts, 1)
            .Set(x => x.LastError, error)
            .Set(x => x.LockedUntil, null)
            .Set(x => x.LockedBy, null);

        update = nextAttemptAt.HasValue
            ? update.Set(x => x.NextAttemptAt, nextAttemptAt.Value)
            : update.Set(x => x.Status, OutboxStatus.Failed).Set(x => x.FailedAt, SystemClock.Instance.GetCurrentInstant());

        return this.Collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }
}

/// <summary>
/// En qué punto está un mensaje de la bandeja.
/// </summary>
/// <remarks>
/// Se guarda como texto para que quien mire la colección lo lea sin tabla de códigos.
/// </remarks>
internal enum OutboxStatus
{
    /// <summary>
    /// Espera a ser enviado, por primera vez o para un reintento.
    /// </summary>
    Pending,

    /// <summary>
    /// Ya salió al bus.
    /// </summary>
    Sent,

    /// <summary>
    /// Agotó sus intentos. No se vuelve a enviar solo: para reenviarlo, se pasa a <see cref="Pending"/> con
    /// <see cref="OutboxDocument.Attempts"/> en cero.
    /// </summary>
    Failed
}

/// <summary>
/// El documento de un mensaje de la bandeja.
/// </summary>
internal sealed class OutboxDocument
{
    /// <summary>
    /// El identificador del evento.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// La clave del evento, que es su destino.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// El nombre de la clase del evento.
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// El agregado que originó el evento.
    /// </summary>
    public Guid AggregateId { get; set; }

    /// <summary>
    /// El evento serializado en JSON, tal como viajará.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Las cabeceras de trazas con las que se encoló.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// En qué punto está.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public OutboxStatus Status { get; set; }

    /// <summary>
    /// Cuántos envíos fallidos lleva.
    /// </summary>
    public int Attempts { get; set; }

    /// <summary>
    /// Por qué falló el último envío.
    /// </summary>
    public string LastError { get; set; }

    /// <summary>
    /// Cuándo se encoló.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// Desde cuándo se puede enviar: al encolarlo, en el acto; tras un fallo, cuando toca el reintento.
    /// </summary>
    public Instant NextAttemptAt { get; set; }

    /// <summary>
    /// Hasta cuándo lo tiene reclamado una réplica, o nulo si nadie.
    /// </summary>
    public Instant? LockedUntil { get; set; }

    /// <summary>
    /// Quién lo tiene reclamado.
    /// </summary>
    public string LockedBy { get; set; }

    /// <summary>
    /// Cuándo se envió. Lo lee el índice TTL que borra lo enviado.
    /// </summary>
    public Instant? SentAt { get; set; }

    /// <summary>
    /// Cuándo agotó sus intentos.
    /// </summary>
    public Instant? FailedAt { get; set; }

    /// <summary>
    /// Crea el documento de un mensaje recién encolado.
    /// </summary>
    /// <param name="message">El mensaje.</param>
    /// <returns>El documento, pendiente y listo para enviar.</returns>
    public static OutboxDocument From(OutboxMessage message) => new()
    {
        Id = message.Id,
        Topic = message.Topic,
        EventName = message.EventName,
        AggregateId = message.AggregateId,
        Body = message.Body,
        Headers = message.Headers ?? [],
        Status = OutboxStatus.Pending,
        Attempts = message.Attempts,
        CreatedAt = message.CreatedAt,
        NextAttemptAt = message.CreatedAt
    };

    /// <summary>
    /// Devuelve el mensaje que guarda el documento.
    /// </summary>
    /// <returns>El mensaje.</returns>
    public OutboxMessage ToMessage() => new(this.Id, this.Topic, this.EventName, this.AggregateId, this.Body, this.Headers, this.CreatedAt, this.Attempts);
}
