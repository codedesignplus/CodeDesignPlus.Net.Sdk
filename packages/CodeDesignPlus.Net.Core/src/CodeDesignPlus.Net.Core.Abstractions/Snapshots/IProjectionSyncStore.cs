namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// Dónde la copia recuerda si ya recibió la foto completa de cada fuente.
/// </summary>
/// <remarks>
/// <c>CodeDesignPlus.Net.Mongo</c> trae una implementación (colección <c>ProjectionSyncState</c> en la base del micro).
/// Sin almacén registrado, la copia no pide fotos al arrancar y lo avisa en el log.
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
    /// Anota que el dueño terminó de publicar la foto completa.
    /// </summary>
    /// <param name="key">La foto, con su aplicación dueña.</param>
    /// <param name="requestId">El identificador de la petición contestada.</param>
    /// <param name="count">Cuántas fotos publicó el dueño.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkSyncedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken);
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
    /// Si la copia ya recibió una foto completa.
    /// </summary>
    public bool IsSynced => this.SyncedAt.HasValue;

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
}
