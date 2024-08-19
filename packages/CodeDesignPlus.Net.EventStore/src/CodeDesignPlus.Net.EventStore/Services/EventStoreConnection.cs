namespace CodeDesignPlus.Net.EventStore.Services;

/// <summary>
/// Represents a connection to the EventStore.
/// </summary>
/// <remarks>
/// This class is responsible for initializing and managing the connection to the EventStore.
/// </remarks>
public class EventStoreConnection : IEventStoreConnection
{
    private readonly CoreOptions coreOptions;
    private readonly ILogger<EventStoreConnection> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConnection"/> class.
    /// </summary>
    /// <param name="coreOptions">The core options.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="coreOptions"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EventStoreConnection(IOptions<CoreOptions> coreOptions, ILogger<EventStoreConnection> logger)
    {
        ArgumentNullException.ThrowIfNull(coreOptions);
        ArgumentNullException.ThrowIfNull(logger);

        this.coreOptions = coreOptions.Value;
        this.logger = logger;

        this.logger.LogInformation("EventStoreConnection initialized.");
    }

    /// <summary>
    /// Initializes the EventStore connection asynchronously.
    /// </summary>
    /// <param name="server">The server configuration.</param>
    /// <returns>The initialized EventStore connection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="server"/> is null.</exception>
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
    /// Handles the authentication failed event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments containing the reason for failure.</param>
    private void AuthenticationFailed(object sender, ES.ClientAuthenticationFailedEventArgs e)
    {
        this.logger.LogError("Authentication failed in EventStore: {reason}", e.Reason);
    }

    /// <summary>
    /// Handles the error occurred event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments containing the exception.</param>
    private void ErrorOccurred(object sender, ES.ClientErrorEventArgs e)
    {
        this.logger.LogError(e.Exception, "Error occurred in EventStore: {exception}", e.Exception.Message);
    }

    /// <summary>
    /// Handles the connection closed event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments containing the reason for closure.</param>
    private void Closed(object sender, ES.ClientClosedEventArgs e)
    {
        this.logger.LogInformation("EventStore connection closed: {reason}", e.Reason);
    }

    /// <summary>
    /// Handles the reconnecting event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Reconnecting(object sender, ES.ClientReconnectingEventArgs e)
    {
        this.logger.LogInformation("Reconnecting to EventStore");
    }

    /// <summary>
    /// Handles the disconnected event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Disconnected(object sender, ES.ClientConnectionEventArgs e)
    {
        this.logger.LogInformation("EventStore connection disconnected.");
    }

    /// <summary>
    /// Handles the connected event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Connected(object sender, ES.ClientConnectionEventArgs e)
    {
        this.logger.LogInformation("EventStore connection established.");
    }
}