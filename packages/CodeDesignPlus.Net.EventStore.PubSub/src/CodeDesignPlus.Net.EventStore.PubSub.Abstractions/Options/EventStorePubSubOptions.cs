using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;

/// <summary>
/// Options to setting of the EventStore.PubSub
/// </summary>
public class EventStorePubSubOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "EventStorePubSub";

    /// <summary>
    /// Gets or sets the name of the connection to the EventStore
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the name of the connection to the EventStore
    /// </summary>
    [Required]
    public string Group { get; set; }
}
