namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the metadata of the event.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The identifier of the event.
    /// </summary>
    Guid EventId { get; }
    /// <summary>
    /// The date and time the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }
    /// <summary>
    /// The identifier of the aggregate root that generated the event.
    /// </summary>
    Guid AggregateId { get; }
    /// <summary>
    /// The metadata of the event.
    /// </summary>
    Dictionary<string, object> Metadata { get; }
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    string EventType { get; }
}
