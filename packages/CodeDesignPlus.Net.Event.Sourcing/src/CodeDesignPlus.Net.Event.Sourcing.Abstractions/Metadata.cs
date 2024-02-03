namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

/// <summary>
/// Represents the metadata of the event.
/// </summary>
public class Metadata(Guid aggregateId, long version, Guid userId, string category)
{
    /// <summary>
    /// The identifier of the aggregate root that generated the event.
    /// </summary>
    public Guid AggregateId { get; private set; } = aggregateId;
    /// <summary>
    /// The version of the aggregate root.
    /// </summary>
    public long Version { get; private set; } = version;
    /// <summary>
    /// The identifier of the user who made the change.
    /// </summary>
    public Guid UserId { get; private set; } = userId;
    /// <summary>
    /// The category of the aggregate root.
    /// </summary>
    public string Category { get; private set; } = category;
}