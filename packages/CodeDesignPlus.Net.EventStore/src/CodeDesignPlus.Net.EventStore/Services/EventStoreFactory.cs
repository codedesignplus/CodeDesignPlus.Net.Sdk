

namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Provides a factory to manage and retrieve connections to EventStore instances.
/// This class ensures that connections are created, cached, and managed efficiently.
/// </summary>
public class EventStoreFactory : IEventStoreFactory
{
    private readonly ConcurrentDictionary<string, ES.IEventStoreConnection> connections = new();
    private readonly IEventStoreConnection eventStoreConnection;
    private readonly ILogger<EventStoreFactory> logger;
    private readonly EventStoreOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreFactory"/> class.
    /// </summary>
    /// <param name="eventStoreConnection">The handler to initialize EventStore connections.</param>
    /// <param name="logger">The logger used for logging events and issues related to the EventStore connections.</param>
    /// <param name="options">The configuration options for the EventStore connections.</param>
    public EventStoreFactory(IEventStoreConnection eventStoreConnection, ILogger<EventStoreFactory> logger, IOptions<EventStoreOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(eventStoreConnection);
        ArgumentNullException.ThrowIfNull(logger);

        this.eventStoreConnection = eventStoreConnection;
        this.logger = logger;
        this.options = options.Value;
    }

    /// <summary>
    /// Creates or retrieves an existing connection to EventStore based on the provided key.
    /// </summary>
    /// <param name="key">The unique identifier representing the desired connection.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the connection to EventStore.</returns>
    /// <exception cref="EventStoreException">Thrown when the specified connection key is not found in the settings.</exception>
    public async Task<ES.IEventStoreConnection> CreateAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (!options.Servers.TryGetValue(key, out Server server))
            throw new EventStoreException("The connection is not registered in the settings.");

        if (connections.TryGetValue(key, out ES.IEventStoreConnection connection))
            return connection;

        connection = await this.eventStoreConnection.InitializeAsync(server).ConfigureAwait(false);
        var succeess = this.connections.TryAdd(key, connection);

        this.logger.LogInformation("Successfully created and cached the connection for key '{key}-{succeess}'.", key, succeess);

        return connection;
    }

    /// <summary>
    /// Attempts to remove the connection with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the connection to be removed.</param>
    /// <returns><c>true</c> if the connection was successfully removed; otherwise, <c>false</c>.</returns>
    public bool RemoveConnection(string key)
    {
        var result = this.connections.TryRemove(key, out _);

        if (result)
            this.logger.LogInformation("Successfully removed the connection for key '{key}'.", key);
        else
            this.logger.LogWarning("Failed to remove the connection for key '{key}'.", key);

        return result;
    }

    /// <summary>
    /// Gets the credentials for the specified connection key.
    /// </summary>
    /// <param name="key">The key connection</param>
    /// <returns>A tuple containing the username and password for the specified connection key.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null or empty.</exception>
    /// <exception cref="EventStoreException">The <paramref name="key"/> is not registered in the settings.</exception>
    public (string, string) GetCredentials(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (!options.Servers.TryGetValue(key, out Server server))
            throw new EventStoreException("The connection is not registered in the settings.");

        return (server.User, server.Password);
    }
}
