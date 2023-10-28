namespace CodeDesignPlus.Net.EventStore.Abstractions;

/// <summary>
/// Represents the details required to connect to an EventStore server or node.
/// </summary>
public class Server
{
    /// <summary>
    /// Gets or sets the connection string as a URI that points to the EventStore server or node.
    /// This is typically used to define the address and port of the EventStore instance.
    /// </summary>
    /// <value>
    /// The URI representing the connection string to the EventStore server.
    /// </value>
    public Uri ConnectionString { get; set; }
}