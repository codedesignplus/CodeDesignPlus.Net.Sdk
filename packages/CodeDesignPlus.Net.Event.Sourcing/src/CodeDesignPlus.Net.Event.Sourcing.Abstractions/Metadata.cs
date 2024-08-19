namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

/// <summary>
/// Represents the metadata of the event.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    /// <param name="aggregateId">The identifier of the aggregate root that generated the event.</param>
    /// <param name="version">The version of the aggregate root.</param>
    /// <param name="userId">The identifier of the user who made the change.</param>
    /// <param name="category">The category of the aggregate root.</param>
    public Metadata(Guid aggregateId, long version, Guid userId, string category)
    {
        AggregateId = aggregateId;
        Version = version;
        UserId = userId;
        Category = category;
    }

    /// <summary>
    /// Gets the identifier of the aggregate root that generated the event.
    /// </summary>
    public Guid AggregateId { get; private set; }

    /// <summary>
    /// Gets the version of the aggregate root.
    /// </summary>
    public long Version { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who made the change.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the category of the aggregate root.
    /// </summary>
    public string Category { get; private set; }
}