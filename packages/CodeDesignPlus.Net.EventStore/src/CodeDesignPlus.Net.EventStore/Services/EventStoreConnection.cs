using ES = EventStore.ClientAPI;
using CodeDesignPlus.Net.Core.Options;
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
        if (coreOptions == null)
            throw new ArgumentNullException(nameof(coreOptions));

        this.coreOptions = coreOptions.Value;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.logger.LogInformation("EventStoreConnection initialized.");
    }

    /// <summary>
    /// Initializes the connection to the EventStore using the provided server details and the internal core options.
    /// </summary>
    /// <param name="server">The server details, including connection string, required to connect to the EventStore.</param>
    /// <returns>A task representing the asynchronous initialization operation. The task result contains the initialized connection.</returns>
    public async Task<ES.IEventStoreConnection> InitializeAsync(Server server)
    {
        if (server == null)
            throw new ArgumentNullException(nameof(server));

        var settings = ES.ConnectionSettings.Create()
            .DisableTls() // Utilizado porque en el docker-compose deshabilitamos la seguridad
            .UseConsoleLogger()
            .KeepReconnecting()
            .KeepRetrying();

        var connection = ES.EventStoreConnection.Create(settings, server.ConnectionString, this.coreOptions.AppName);

        try
        {
            await connection.ConnectAsync();
            this.logger.LogInformation("Successfully connected to EventStore.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error while connecting to EventStore.");
            throw;
        }

        return connection;
    }
}
