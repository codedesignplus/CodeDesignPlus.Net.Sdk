namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// Dónde la copia recuerda si ya recibió la foto completa de cada fuente, y cuántas fotos de cada petición aplicó.
/// </summary>
/// <remarks>
/// <c>CodeDesignPlus.Net.Mongo</c> trae una implementación (colecciones <c>ProjectionSyncState</c>,
/// <c>ProjectionSnapshotProgress</c> y <c>ProjectionSnapshotReceipt</c> en la base del micro). Sin almacén registrado,
/// la copia no pide fotos al arrancar y lo avisa en el log.
/// <para>
/// Una foto queda sincronizada solo cuando la copia aplicó con éxito tantas fotos como anunció el dueño al cerrar
/// (pendings/485): el cierre y las fotos llegan por colas distintas y en cualquier orden, así que cada lado anota lo suyo
/// en el progreso de la petición y lee lo del otro en la misma operación atómica.
/// </para>
/// </remarks>
public interface IProjectionSyncStore
{
    /// <summary>
    /// Lee el estado de sincronización de una foto.
    /// </summary>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El estado, o nulo si nunca se pidió.</returns>
    Task<ProjectionSyncState?> FindAsync(SnapshotKey key, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que se pidió la foto completa.
    /// </summary>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="requestId">El identificador de la petición.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkRequestedAsync(SnapshotKey key, Guid requestId, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que la copia aplicó todas las fotos de una petición completa: la fuente queda sincronizada.
    /// </summary>
    /// <remarks>
    /// Borra el progreso y los recibos de esa foto, que ya no hacen falta: una fuente sincronizada no se vuelve a pedir.
    /// </remarks>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="requestId">El identificador de la petición contestada.</param>
    /// <param name="count">Cuántas fotos publicó el dueño, y aplicó la copia.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkSyncedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que el dueño cerró una petición completa y la copia todavía no aplicó todas sus fotos.
    /// </summary>
    /// <remarks>
    /// No quita una sincronización anterior: solo deja constancia de qué petición quedó pendiente, para avisar y volver a
    /// pedirla si no se completa.
    /// </remarks>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="requestId">El identificador de la petición cerrada.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkIncompleteAsync(SnapshotKey key, Guid requestId, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que la copia aplicó con éxito una foto de una petición.
    /// </summary>
    /// <remarks>
    /// Idempotente por mensaje: el bus puede reentregar la misma foto, y solo cuenta la primera vez.
    /// </remarks>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="requestId">La petición que contesta la foto.</param>
    /// <param name="eventId">El identificador del mensaje de la foto.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El progreso de la petición después de contarla, o nulo si esa foto ya estaba contada.</returns>
    Task<SnapshotProgress?> RecordAppliedAsync(SnapshotKey key, Guid requestId, Guid eventId, CancellationToken cancellationToken);

    /// <summary>
    /// Anota cuántas fotos anunció el dueño al cerrar una petición.
    /// </summary>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="requestId">La petición cerrada.</param>
    /// <param name="count">Cuántas fotos publicó el dueño.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El progreso de la petición después de anotar el cierre.</returns>
    Task<SnapshotProgress> RecordCompletedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken);

    /// <summary>
    /// Lee el progreso de una petición.
    /// </summary>
    /// <param name="requestId">La petición.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El progreso, o nulo si no se aplicó ninguna foto ni llegó el cierre.</returns>
    Task<SnapshotProgress?> FindProgressAsync(Guid requestId, CancellationToken cancellationToken);
}

/// <summary>
/// Cuántas fotos de una petición aplicó la copia y cuántas anunció el dueño al cerrarla.
/// </summary>
/// <param name="RequestId">La petición.</param>
/// <param name="Applied">Cuántas fotos aplicó la copia con éxito.</param>
/// <param name="Expected">Cuántas anunció el dueño al cerrar, o nulo si el cierre no ha llegado.</param>
public sealed record SnapshotProgress(Guid RequestId, long Applied, long? Expected)
{
    /// <summary>
    /// El dueño cerró la petición y la copia aplicó exactamente lo que anunció.
    /// </summary>
    /// <remarks>
    /// Igualdad y no «al menos»: cada foto cuenta una sola vez por mensaje, así que aplicar más de lo anunciado solo pasa si
    /// el dueño repitió la petición a medias, y entonces la cuenta no prueba nada.
    /// </remarks>
    public bool IsComplete => this.Expected.HasValue && this.Applied == this.Expected.Value;
}

/// <summary>
/// El estado de sincronización de una foto en una copia.
/// </summary>
/// <param name="Source">La aplicación dueña.</param>
/// <param name="Aggregate">El nombre del agregado.</param>
/// <param name="Snapshot">El nombre del evento de foto.</param>
/// <param name="LastRequestId">La última petición publicada.</param>
/// <param name="RequestedAt">Cuándo se publicó la última petición.</param>
/// <param name="SyncedAt">Cuándo terminó la última foto completa, o nulo si ninguna ha terminado.</param>
/// <param name="Count">Cuántas fotos trajo la última foto completa.</param>
public sealed record ProjectionSyncState(string Source, string Aggregate, string Snapshot, Guid? LastRequestId, Instant? RequestedAt, Instant? SyncedAt, long Count)
{
    private static readonly Guid Namespace = Guid.Parse("6a3f0b1e-5c2d-4e8f-9a7b-380380380380");

    /// <summary>
    /// Cuántas fotos de la foto completa aplicó la copia con éxito, o nulo si la marca es anterior a que se contaran
    /// (pendings/485).
    /// </summary>
    public long? Applied { get; init; }

    /// <summary>
    /// La última petición que el dueño cerró sin que la copia hubiera aplicado todas sus fotos.
    /// </summary>
    public Guid? IncompleteRequestId { get; init; }

    /// <summary>
    /// Cuándo cerró el dueño esa petición.
    /// </summary>
    public Instant? IncompleteAt { get; init; }

    /// <summary>
    /// Si la copia ya recibió y aplicó una foto completa.
    /// </summary>
    /// <remarks>
    /// Una marca sin <see cref="Applied"/> se puso antes de que se contaran las fotos aplicadas, solo con la cuenta del
    /// dueño: no prueba que la copia esté completa y se vuelve a pedir.
    /// </remarks>
    public bool IsSynced => this.SyncedAt.HasValue && this.Applied.HasValue;

    /// <summary>
    /// Si el dueño cerró una petición y la copia no ha aplicado todas sus fotos.
    /// </summary>
    public bool IsIncomplete => !this.IsSynced && this.IncompleteAt.HasValue;

    /// <summary>
    /// El identificador del documento de estado de una foto: derivado de la aplicación dueña y el nombre de la foto, así
    /// dos réplicas que lo crean a la vez escriben el mismo documento.
    /// </summary>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <returns>El identificador determinista.</returns>
    public static Guid IdOf(SnapshotKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        return DeterministicGuid.Create(Namespace, $"{key.Source}/{key.Snapshot}".ToLowerInvariant());
    }

    /// <summary>
    /// El identificador del recibo de una foto aplicada: derivado de la petición y del mensaje, así una reentrega del bus
    /// choca con el recibo de la primera entrega y no cuenta dos veces.
    /// </summary>
    /// <param name="requestId">La petición que contesta la foto.</param>
    /// <param name="eventId">El identificador del mensaje de la foto.</param>
    /// <returns>El identificador determinista.</returns>
    public static Guid ReceiptIdOf(Guid requestId, Guid eventId) => DeterministicGuid.Create(Namespace, $"{requestId}/{eventId}");
}
