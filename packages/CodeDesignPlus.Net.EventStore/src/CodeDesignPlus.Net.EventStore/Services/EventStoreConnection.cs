using CodeDesignPlus.Net.Core.Abstractions.Options;
using ES = EventStore.ClientAPI;
namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Provides an implementation for connecting to an EventStore instance using specified configuration options.
/// This class encapsulates the logic for setting up and initializing the connection to EventStore.
/// </summary>
public class EventStoreConnection : IEventStoreConnection
{
    private readonly CoreOptions coreOptions;
    private readonly ILogger<EventStoreConnection> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConnection"/> class.
    /// </summary>
    /// <param name="coreOptions">The core options required for the connection, fetched from the application's configuration.</param>
    /// <param name="logger">The logger used for logging events and issues related to the EventStore connection.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="coreOptions"/> parameter is null.</exception>
    public EventStoreConnection(IOptions<CoreOptions> coreOptions, ILogger<EventStoreConnection> logger)
    {
        ArgumentNullException.ThrowIfNull(coreOptions);
        ArgumentNullException.ThrowIfNull(logger);

        this.coreOptions = coreOptions.Value;
        this.logger = logger;

        this.logger.LogInformation("EventStoreConnection initialized.");
    }

    /// <summary>
    /// Initializes the connection to the EventStore using the provided server details and the internal core options.
    /// </summary>
    /// <param name="server">The server details, including connection string, required to connect to the EventStore.</param>
    /// <returns>A task representing the asynchronous initialization operation. The task result contains the initialized connection.</returns>
    public async Task<ES.IEventStoreConnection> InitializeAsync(Server server)
    {
        ArgumentNullException.ThrowIfNull(server);

        var settings = ES.ConnectionSettings.Create()
            .SetClusterGossipPort(server.ConnectionString.Port)
            .DisableTls()
            .UseConsoleLogger()
            .KeepReconnecting()
            .KeepRetrying();

        var connection = ES.EventStoreConnection.Create(settings, server.ConnectionString, this.coreOptions.AppName);

        connection.Connected += Connected;
        connection.Disconnected += Disconnected;
        connection.Reconnecting += Reconnecting;
        connection.Closed += Closed;
        connection.ErrorOccurred += ErrorOccurred;
        connection.AuthenticationFailed += AuthenticationFailed;

        await connection.ConnectAsync().ConfigureAwait(false);

        this.logger.LogInformation("Successfully connected to EventStore.");

        return connection;
    }
    
    /// <summary>
    /// Handles authentication failure events from the EventStore client.
    /// Logs the authentication failure reason using ILogger.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments containing the reason for authentication failure.</param>
    private void AuthenticationFailed(object sender, ES.ClientAuthenticationFailedEventArgs e)
    {
        this.logger.LogError("Authentication failed in EventStore: {reason}", e.Reason);
    }

    /// <summary>
    /// Handles error events from the EventStore client.
    /// Logs the exception message using ILogger.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments containing the exception that occurred.</param>
    private void ErrorOccurred(object sender, ES.ClientErrorEventArgs e)
    {
        this.logger.LogError(e.Exception, "Error occurred in EventStore: {exception}", e.Exception.Message);
    }

    /// <summary>
    /// Handles the closed event of the EventStore client connection.
    /// Logs the reason for connection closure using ILogger.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments containing the reason for connection closure.</param>
    private void Closed(object sender, ES.ClientClosedEventArgs e)
    {
        this.logger.LogInformation("EventStore connection closed: {reason}", e.Reason);
    }

    /// <summary>
    /// Handles the reconnecting event of the EventStore client.
    /// Logs a message indicating reconnection attempt using ILogger.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments containing details about the reconnection attempt.</param>
    private void Reconnecting(object sender, ES.ClientReconnectingEventArgs e)
    {
        this.logger.LogInformation("Reconnecting to EventStore");
    }

    /// <summary>
    /// Handles the disconnected event of the EventStore client connection.
    /// Logs a message indicating that the connection has been disconnected using ILogger.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments containing details about the disconnection.</param>
    private void Disconnected(object sender, ES.ClientConnectionEventArgs e)
    {
        this.logger.LogInformation("EventStore connection disconnected.");
    }

    /// <summary>
    /// Handles the connected event of the EventStore client connection.
    /// Logs a message indicating that the connection has been established using ILogger.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments containing details about the connection.</param>
    private void Connected(object sender, ES.ClientConnectionEventArgs e)
    {
        this.logger.LogInformation("EventStore connection established.");
    }

}
