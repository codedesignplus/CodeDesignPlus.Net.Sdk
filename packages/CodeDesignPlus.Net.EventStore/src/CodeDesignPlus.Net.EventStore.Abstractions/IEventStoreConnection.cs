namespace CodeDesignPlus.Net.EventStore.Abstractions;

/// <summary>
/// Defines the contract for connecting and interacting with an EventStore instance.
/// </summary>
public interface IEventStoreConnection
{
    /// <summary>
    /// Initializes the connection to the EventStore using the provided server details.
    /// </summary>
    /// <param name="server">The server details, including connection string, required to connect to the EventStore.</param>
    /// <returns>A task representing the asynchronous initialization operation. The task result contains the initialized EventStore connection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="server"/> is null.</exception>
    Task<ES.IEventStoreConnection> InitializeAsync(Server server);
}