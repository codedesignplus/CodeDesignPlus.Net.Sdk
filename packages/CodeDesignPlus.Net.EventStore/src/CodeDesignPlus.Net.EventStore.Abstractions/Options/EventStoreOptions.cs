using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.EventStore.Abstractions.Options;

/// <summary>
/// Options to setting of the EventStore
/// </summary>
public class EventStoreOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "EventStore";

    /// <summary>
    /// Gets or sets the collection of EventStore servers (nodes) to which the application can connect.
    /// Each server is represented by a key-value pair, where the key is a unique identifier or name for the server,
    /// and the value contains the server's connection details.
    /// </summary>
    /// <value>
    /// A dictionary of servers where the key is a unique identifier or name for the server, 
    /// and the value contains the server's connection details.
    /// </value>
    public Dictionary<string, Server> Servers { get; set; } = new();
}
