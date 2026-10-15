using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// Una copia local le pide a su dueño el estado de todos sus agregados de un tipo, o los de una sola copropiedad.
/// </summary>
/// <remarks>
/// Es un contrato de plataforma, no un evento de quien lo publica: la clave es fija, así que todas las aplicaciones
/// publican y escuchan en el mismo tema, y cada dueño descarta lo que no nombra su <c>Core:AppName</c> en
/// <see cref="Source"/>. No declare una copia en un micro: con otra clave publicaría a un tema que nadie lee.
/// <para>
/// La publica el SDK al arrancar la copia mientras la fuente no figure como sincronizada, y la contesta el SDK del dueño
/// con un evento de foto por agregado y un <see cref="SnapshotCompletedDomainEvent"/> al final.
/// </para>
/// </remarks>
/// <param name="aggregateId">El identificador de la petición (el mismo que <paramref name="requestId"/>).</param>
/// <param name="source">La aplicación dueña del agregado (su <c>Core:AppName</c>).</param>
/// <param name="aggregate">El nombre del agregado, igual a la entidad de la clave de su evento de foto.</param>
/// <param name="requestId">El identificador de la petición; viaja en cada foto y en el cierre.</param>
/// <param name="requestedBy">La aplicación que pide (su <c>Core:AppName</c>).</param>
/// <param name="tenant">La copropiedad a fotografiar, o nulo para todas.</param>
/// <param name="eventId">El identificador del evento (opcional).</param>
/// <param name="occurredAt">Cuándo se pidió (opcional).</param>
/// <param name="metadata">Metadatos adicionales (opcional).</param>
[EventKey("ProjectionSnapshot", 1, "SnapshotRequestedDomainEvent", "ms-snapshots", "codedesignplus")]
public class SnapshotRequestedDomainEvent(
    Guid aggregateId,
    string source,
    string aggregate,
    Guid requestId,
    string requestedBy,
    Guid? tenant,
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
    /// El nombre del agregado pedido.
    /// </summary>
    public string Aggregate { get; private set; } = aggregate;

    /// <summary>
    /// El identificador de la petición.
    /// </summary>
    public Guid RequestId { get; private set; } = requestId;

    /// <summary>
    /// La aplicación que pide.
    /// </summary>
    public string RequestedBy { get; private set; } = requestedBy;

    /// <summary>
    /// La copropiedad a fotografiar, o nulo para todas.
    /// </summary>
    public Guid? Tenant { get; private set; } = tenant;

    /// <summary>
    /// Crea la petición.
    /// </summary>
    /// <param name="source">La aplicación dueña del agregado.</param>
    /// <param name="aggregate">El nombre del agregado.</param>
    /// <param name="requestedBy">La aplicación que pide.</param>
    /// <param name="tenant">La copropiedad a fotografiar, o nulo para todas.</param>
    /// <returns>Un nuevo <see cref="SnapshotRequestedDomainEvent"/> con un identificador de petición nuevo.</returns>
    public static SnapshotRequestedDomainEvent Create(string source, string aggregate, string requestedBy, Guid? tenant)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(aggregate);
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedBy);

        var requestId = Guid.NewGuid();

        return new SnapshotRequestedDomainEvent(requestId, source, aggregate, requestId, requestedBy, tenant);
    }
}
