using ES = EventStore.ClientAPI;

namespace CodeDesignPlus.Net.EventStore.Abstractions;

/// <summary>
/// Defines a contract for managing and retrieving connections to EventStore instances.
/// </summary>
public interface IEventStoreFactory
{
    /// <summary>
    /// Creates or retrieves an existing connection to EventStore based on the provided key.
    /// </summary>
    /// <param name="key">The unique identifier representing the desired connection.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the connection to EventStore.</returns>
    Task<ES.IEventStoreConnection> CreateAsync(string key);

    /// <summary>
    /// Attempts to remove the connection with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the connection to be removed.</param>
    /// <returns><c>true</c> if the connection was successfully removed; otherwise, <c>false</c>.</returns>
    bool RemoveConnection(string key);

    /// <summary>
    /// Gets the credentials for the specified connection key.
    /// </summary>
    /// <param name="key">The key connection</param>
    /// <returns>A tuple containing the username and password for the specified connection key.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null or empty.</exception>
    /// <exception cref="EventStoreException">The <paramref name="key"/> is not registered in the settings.</exception>
    (string, string) GetCredentials(string key);
}
