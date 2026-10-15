namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// Base de los eventos de foto: el estado completo de un agregado, tal como lo tiene su dueño, enviado a la copia que lo
/// pidió con un <see cref="SnapshotRequestedDomainEvent"/>.
/// </summary>
/// <remarks>
/// Cada pareja dueño-copia declara su evento de foto como cualquier otro evento: el dueño con
/// <c>[EventKey&lt;TAggregate&gt;(1, "XSnapshotDomainEvent")]</c> y la copia con su gemelo, que nombra al dueño en la clave.
/// La entidad de la clave es el nombre del agregado y la aplicación es el dueño: con eso la copia sabe a quién pedirle.
/// <para>
/// <see cref="DomainEvent.OccurredAt"/> es el momento en que el dueño leyó el agregado. Una copia que ya aplicó un evento
/// en vivo posterior no debe pisarlo con la foto: <see cref="Supersedes"/> lo decide.
/// </para>
/// <para>
/// <see cref="RequestId"/> y <see cref="RequestedBy"/> los pone el SDK al publicar la foto. Van con <c>set</c> público
/// porque el bus deserializa con Newtonsoft y un <c>set</c> privado que no está en el constructor del gemelo no vuelve.
/// </para>
/// </remarks>
/// <param name="aggregateId">El identificador del agregado fotografiado.</param>
/// <param name="tenant">La copropiedad (o el inquilino) dueño del agregado.</param>
/// <param name="eventId">El identificador del evento (opcional).</param>
/// <param name="occurredAt">Cuándo se leyó el agregado (opcional; por defecto, ahora).</param>
/// <param name="metadata">Metadatos adicionales (opcional).</param>
public abstract class SnapshotDomainEvent(Guid aggregateId, Guid tenant, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata), ITenant
{
    /// <summary>
    /// La copropiedad (o el inquilino) dueño del agregado.
    /// </summary>
    public Guid Tenant { get; private set; } = tenant;

    /// <summary>
    /// La petición que contesta esta foto.
    /// </summary>
    public Guid RequestId { get; set; }

    /// <summary>
    /// La aplicación que pidió la foto (su <c>Core:AppName</c>). Las demás copias del mismo agregado la descartan.
    /// </summary>
    public string? RequestedBy { get; set; }

    /// <summary>
    /// Indica si la foto es más nueva que lo último que la copia aplicó del dueño.
    /// </summary>
    /// <param name="sourceChangedAt">El <c>OccurredAt</c> del último evento del dueño aplicado a la copia, o nulo si la copia no existe o no lo guarda.</param>
    /// <returns><c>true</c> si la foto se puede aplicar sin perder un cambio en vivo más nuevo.</returns>
    public bool Supersedes(Instant? sourceChangedAt) => sourceChangedAt is null || this.OccurredAt >= sourceChangedAt.Value;

    /// <summary>
    /// Indica si la foto va dirigida a la aplicación indicada.
    /// </summary>
    /// <param name="appName">El <c>Core:AppName</c> de quien la recibe.</param>
    /// <returns><c>true</c> si la pidió esa aplicación, o si no dice quién la pidió.</returns>
    public bool IsAddressedTo(string appName) => string.IsNullOrWhiteSpace(this.RequestedBy) || string.Equals(this.RequestedBy, appName, StringComparison.OrdinalIgnoreCase);
}
