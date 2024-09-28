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
    [Required]
    public Uri ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the user name to use when connecting to the EventStore server.
    /// </summary>
    /// <value>
    /// The user name for the EventStore server connection.
    /// </value>
    [Required]
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the password to use when connecting to the EventStore server.
    /// </summary>
    /// <value>
    /// The password for the EventStore server connection.
    /// </value>
    [Required]
    public string Password { get; set; }
}