namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the domain event that will be used to notify the changes that occur in the domain.
/// </summary>
/// <param name="aggregateId">The identifier of the aggregate root that generated the event.</param>
/// <param name="eventId">The identifier of the event.</param>
/// <param name="occurredAt">The date and time when the event occurred.</param>
/// <param name="metadata">The metadata of the event.</param>
public abstract class DomainEvent(
    Guid aggregateId,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : IDomainEvent
{
    /// <summary>
    /// The identifier of the aggregate root that generated the event.
    /// </summary>
    public Guid AggregateId { get; private set; } = aggregateId;
    /// <summary>
    /// The identifier of the event.
    /// </summary>
    public Guid EventId { get; internal set; } = eventId ?? Guid.NewGuid();
    /// <summary>
    /// The date and time when the event occurred.
    /// </summary>   
    public DateTime OccurredAt { get; internal set; } = occurredAt ?? DateTime.UtcNow;
    /// <summary>
    /// The metadata of the event.
    /// </summary>    
    public Dictionary<string, object> Metadata { get; internal set; } = metadata ?? [];

    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    public string EventType
    {
        get
        {
            var attribute = this.GetType().GetCustomAttribute<EventKeyAttribute>();

            if (attribute is null)
                throw new InvalidOperationException($"The event {this.GetType().Name} does not have the {nameof(EventKeyAttribute)} attribute.");

            return attribute.Key;
        }
    }
}