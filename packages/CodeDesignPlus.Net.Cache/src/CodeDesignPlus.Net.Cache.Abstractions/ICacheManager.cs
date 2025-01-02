namespace CodeDesignPlus.Net.Cache.Abstractions;

/// <summary>
/// Service to manage the cache.
/// </summary>
public interface ICacheManager
{
    /// <summary>
    /// Asynchronously retrieves a value from the cache based on the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached value, or the default value of T if not found.</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// Asynchronously sets a value in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The key of the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiration">Optional. The expiration time for the cached value. If null, the value will not expire.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Asynchronously removes a value from the cache based on the specified key.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Asynchronously checks if a value exists in the cache.
    /// </summary>
    /// <param name="key">The key of the value to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the value exists in the cache, false otherwise.</returns>
    Task<bool> ExistsAsync(string key);

   /// <summary>
    /// Asynchronously clears or empties the entire cache.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ClearAsync();
}