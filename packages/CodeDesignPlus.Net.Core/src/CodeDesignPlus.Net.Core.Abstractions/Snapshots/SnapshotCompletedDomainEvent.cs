using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// El dueño terminó de publicar las fotos de una petición.
/// </summary>
/// <remarks>
/// Contrato de plataforma con clave fija, como <see cref="SnapshotRequestedDomainEvent"/>. Lo publica el SDK del dueño
/// después de la última foto; el SDK de la copia que pidió marca la fuente como sincronizada si la petición era de todas
/// las copropiedades. Las demás aplicaciones lo descartan.
/// <para>
/// El bus no garantiza orden: el cierre puede procesarse antes que las últimas fotos. No importa, porque las fotos ya
/// están en la cola durable de la copia y se aplican igual; la marca solo evita volver a pedir al siguiente arranque.
/// </para>
/// </remarks>
/// <param name="aggregateId">El identificador de la petición.</param>
/// <param name="source">La aplicación dueña del agregado.</param>
/// <param name="aggregate">El nombre del agregado.</param>
/// <param name="requestId">El identificador de la petición contestada.</param>
/// <param name="requestedBy">La aplicación que pidió.</param>
/// <param name="tenant">La copropiedad fotografiada, o nulo si fueron todas.</param>
/// <param name="count">Cuántas fotos se publicaron.</param>
/// <param name="eventId">El identificador del evento (opcional).</param>
/// <param name="occurredAt">Cuándo terminó (opcional).</param>
/// <param name="metadata">Metadatos adicionales (opcional).</param>
[EventKey("ProjectionSnapshot", 1, "SnapshotCompletedDomainEvent", "ms-snapshots", "codedesignplus")]
public class SnapshotCompletedDomainEvent(
    Guid aggregateId,
    string source,
    string aggregate,
    Guid requestId,
    string requestedBy,
    Guid? tenant,
    long count,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    /// <summary>
    /// La aplicación dueña del agregado.
    /// </summary>
    public string Source { get; private set; } = source;

    /// <summary>
    /// El nombre del agregado.
    /// </summary>
    public string Aggregate { get; private set; } = aggregate;

    /// <summary>
    /// El identificador de la petición contestada.
    /// </summary>
    public Guid RequestId { get; private set; } = requestId;

    /// <summary>
    /// La aplicación que pidió.
    /// </summary>
    public string RequestedBy { get; private set; } = requestedBy;

    /// <summary>
    /// La copropiedad fotografiada, o nulo si fueron todas.
    /// </summary>
    public Guid? Tenant { get; private set; } = tenant;

    /// <summary>
    /// Cuántas fotos se publicaron.
    /// </summary>
    public long Count { get; private set; } = count;

    /// <summary>
    /// Crea el cierre de una petición.
    /// </summary>
    /// <param name="request">La petición contestada.</param>
    /// <param name="count">Cuántas fotos se publicaron.</param>
    /// <returns>Un nuevo <see cref="SnapshotCompletedDomainEvent"/>.</returns>
    public static SnapshotCompletedDomainEvent Create(SnapshotRequestedDomainEvent request, long count)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new SnapshotCompletedDomainEvent(request.RequestId, request.Source, request.Aggregate, request.RequestId, request.RequestedBy, request.Tenant, count);
    }
}
