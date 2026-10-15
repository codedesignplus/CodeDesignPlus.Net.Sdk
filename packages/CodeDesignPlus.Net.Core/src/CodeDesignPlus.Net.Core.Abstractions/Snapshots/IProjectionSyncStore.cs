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
    /// Lee el estado de sincronización de una fuente.
    /// </summary>
    /// <param name="source">La aplicación dueña.</param>
    /// <param name="aggregate">El nombre del agregado.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El estado, o nulo si nunca se pidió.</returns>
    Task<ProjectionSyncState?> FindAsync(string source, string aggregate, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que se pidió la foto completa de una fuente.
    /// </summary>
    /// <param name="source">La aplicación dueña.</param>
    /// <param name="aggregate">El nombre del agregado.</param>
    /// <param name="requestId">El identificador de la petición.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkRequestedAsync(string source, string aggregate, Guid requestId, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que el dueño terminó de publicar la foto completa de una fuente.
    /// </summary>
    /// <param name="source">La aplicación dueña.</param>
    /// <param name="aggregate">El nombre del agregado.</param>
    /// <param name="requestId">El identificador de la petición contestada.</param>
    /// <param name="count">Cuántas fotos publicó el dueño.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkSyncedAsync(string source, string aggregate, Guid requestId, long count, CancellationToken cancellationToken);
}

/// <summary>
/// El estado de sincronización de una fuente en una copia.
/// </summary>
/// <param name="Source">La aplicación dueña.</param>
/// <param name="Aggregate">El nombre del agregado.</param>
/// <param name="LastRequestId">La última petición publicada.</param>
/// <param name="RequestedAt">Cuándo se publicó la última petición.</param>
/// <param name="SyncedAt">Cuándo terminó la última foto completa, o nulo si ninguna ha terminado.</param>
/// <param name="Count">Cuántas fotos trajo la última foto completa.</param>
public sealed record ProjectionSyncState(string Source, string Aggregate, Guid? LastRequestId, Instant? RequestedAt, Instant? SyncedAt, long Count)
{
    /// <summary>
    /// Si la copia ya recibió una foto completa de la fuente.
    /// </summary>
    public bool IsSynced => this.SyncedAt.HasValue;

    /// <summary>
    /// El identificador del documento de estado de una fuente: derivado de la fuente y el agregado, así dos réplicas que
    /// lo crean a la vez escriben el mismo documento.
    /// </summary>
    /// <param name="source">La aplicación dueña.</param>
    /// <param name="aggregate">El nombre del agregado.</param>
    /// <returns>El identificador determinista.</returns>
    public static Guid IdOf(string source, string aggregate) => DeterministicGuid.Create(Namespace, $"{source}/{aggregate}".ToLowerInvariant());

    private static readonly Guid Namespace = Guid.Parse("6a3f0b1e-5c2d-4e8f-9a7b-380380380380");
}
