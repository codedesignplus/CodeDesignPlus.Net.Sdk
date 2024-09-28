namespace CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;

/// <summary>
/// Represents the configuration options for EventStore Pub/Sub.
/// </summary>
public class EventStorePubSubOptions : PubSubOptions
{
    /// <summary>
    /// The configuration section name for EventStore Pub/Sub options.
    /// </summary>
    public static new readonly string Section = "EventStorePubSub";

    /// <summary>
    /// Gets or sets a value indicating whether EventStore Pub/Sub is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the group name for the EventStore Pub/Sub subscription.
    /// </summary>
    [Required]
    public string Group { get; set; }
}