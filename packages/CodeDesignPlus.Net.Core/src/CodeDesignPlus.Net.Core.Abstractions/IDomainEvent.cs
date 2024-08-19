namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a domain event.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Gets the unique identifier of the aggregate associated with the event.
    /// </summary>
    Guid AggregateId { get; }

    /// <summary>
    /// Gets the metadata associated with the event.
    /// </summary>
    Dictionary<string, object> Metadata { get; }
}
