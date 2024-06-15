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
        ArgumentNullException.ThrowIfNull(nameof(coreOptions));
        ArgumentNullException.ThrowIfNull(nameof(logger));

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
        ArgumentNullException.ThrowIfNull(nameof(server));

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

    private void AuthenticationFailed(object sender, ES.ClientAuthenticationFailedEventArgs e)
    {
        this.logger.LogError("Authentication failed in EventStore: {reason}", e.Reason);
    }

    private void ErrorOccurred(object sender, ES.ClientErrorEventArgs e)
    {
        this.logger.LogError(e.Exception, "Error occurred in EventStore: {exception}", e.Exception.Message);
    }

    private void Closed(object sender, ES.ClientClosedEventArgs e)
    {
        this.logger.LogInformation("EventStore connection closed: {reason}", e.Reason);
    }

    private void Reconnecting(object sender, ES.ClientReconnectingEventArgs e)
    {
        this.logger.LogInformation("Reconnecting to EventStore");
    }

    private void Disconnected(object sender, ES.ClientConnectionEventArgs e)
    {
        this.logger.LogInformation("EventStore connection disconnected.");
    }

    private void Connected(object sender, ES.ClientConnectionEventArgs e)
    {
        this.logger.LogInformation("EventStore connection established.");
    }
}
