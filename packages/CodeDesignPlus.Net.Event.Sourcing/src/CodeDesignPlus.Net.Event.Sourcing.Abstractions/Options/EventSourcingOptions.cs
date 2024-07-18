namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions.Options;

/// <summary>
/// Options to setting of the Event.Sourcing
/// </summary>
public class EventSourcingOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "EventSourcing";

    /// <summary>
    /// Gets or sets the main name for the aggregate. 
    /// This is used as the central part of the naming pattern for aggregates in the event store.
    /// </summary>
    /// <value>The main name for the aggregate.</value>
    public string MainName { get; set; } = "aggregate";

    /// <summary>
    /// Gets or sets the suffix used to denote snapshots in the naming pattern for aggregates in the event store.
    /// When an aggregate's state is persisted as a snapshot, this suffix is appended to the aggregate's name.
    /// </summary>
    /// <value>The snapshot suffix for the aggregate.</value>
    public string SnapshotSuffix { get; set; } = "snapshot";
    /// <summary>
    /// Gets or sets the frequency at which snapshots are taken for Event Sourcing.
    /// </summary>
    [Range(1, 500)]
    public int FrequencySnapshot { get; set; } = 20;
}
