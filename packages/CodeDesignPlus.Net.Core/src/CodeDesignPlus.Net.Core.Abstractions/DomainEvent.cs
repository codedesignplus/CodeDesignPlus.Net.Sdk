namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a base class for domain events.
/// </summary>
/// <remarks>
/// Domain events are used to represent significant changes or occurrences in a domain model.
/// </remarks>
/// <seealso cref="IDomainEvent"/>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainEvent"/> class.
/// </remarks>
/// <param name="aggregateId">The identifier of the aggregate associated with the event.</param>
/// <param name="eventId">The identifier of the event (optional). If not provided, a new GUID will be generated.</param>
/// <param name="occurredAt">The date and time when the event occurred (optional). If not provided, the current UTC date and time will be used.</param>
/// <param name="metadata">Additional metadata associated with the event (optional).</param>
public abstract class DomainEvent(
    Guid aggregateId,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : IDomainEvent
{

    /// <summary>
    /// Gets the identifier of the aggregate associated with the event.
    /// </summary>
    public Guid AggregateId { get; private set; } = aggregateId;

    /// <summary>
    /// Gets or sets the identifier of the event.
    /// </summary>
    /// <remarks>
    /// This property is internal and can only be set within t
    /// <summary>
    /// Gets or sets the identifier of the event.
    /// </summary>
    /// <remarks>
    /// This property is internal and can only be set within the assembly.
    /// If not provided during initialization, a new GUID will be generated.
    /// </remarks>he assembly.
    /// If not provided during initialization, a new GUID will be generated.
    /// </remarks>
    public Guid EventId { get; internal set; } = eventId ?? Guid.NewGuid();

    /// <summary>
    /// Gets or sets the date and time when the event occurred.
    /// </summary>
    /// <remarks>
    /// This property is internal and can only be set within the assembly.
    /// If not provided during initialization, the current UTC date and time will be used.
    /// </remarks>
    public DateTime OccurredAt { get; internal set; } = occurredAt ?? DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the additional metadata associated with the event.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = metadata ?? [];
}
