using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.Serializers;
using Newtonsoft.Json.Linq;

namespace CodeDesignPlus.Net.Redis.Cache.Services;

/// /// <summary>
/// Manages the interaction with a Redis cache.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RedisCacheManager"/> class.
/// </remarks>
/// <param name="factory">The factory used to create Redis connections.</param>
/// <param name="logger">The logger for logging messages.</param>
/// <param name="options">The options for configuring the Redis cache.</param>
public class RedisCacheManager(IRedisFactory factory, ILogger<RedisCacheManager> logger, IOptions<RedisCacheOptions> options) : IRedisCacheManager
{
    private readonly RedisCacheOptions cacheOptions = options.Value;
    private readonly IRedis redis = factory.Create(FactoryConst.RedisCore);

    /// <summary>
    /// Clears all data from the Redis cache.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The key is <c>null</c> or empty.</exception>
    public Task ClearAsync()
    {
        if (this.redis.Database == null)
        {
            logger.LogWarning("The cache will not be cleared because the connection to the Redis server could not be established");

            return Task.CompletedTask;
        }

        logger.LogWarning("The cache will be cleared");

        return this.redis.Database.ExecuteAsync("FLUSHDB");
    }

    /// <summary>
    /// Checks if a key exists in the Redis cache.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, returning <c>true</c> if the key exists, otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">The key is <c>null</c> or empty.</exception>
    public Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            ArgumentNullException.ThrowIfNull(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {key} could not be verified because the connection to the Redis server could not be established", key);

            return Task.FromResult(false);
        }

        return this.redis.Database.KeyExistsAsync(key);
    }

    /// <summary>
    /// Retrieves a value from the Redis cache based on the provided key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, returning the value associated with the key if found, otherwise <c>default</c>.</returns>
    /// <exception cref="ArgumentNullException">The key is <c>null</c> or empty.</exception>
    public async Task<T> GetAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            ArgumentNullException.ThrowIfNull(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {key} could not be retrieved because the connection to the Redis server could not be established", key);

            return default;
        }

        var data = await this.redis.Database.StringGetAsync(key);

        if (data.IsNullOrEmpty)
        {
            logger.LogDebug("The key {key} does not exist in the cache", key);

            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    /// <summary>
    /// Removes a key and its associated value from the Redis cache.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The key is <c>null</c> or empty.</exception>
    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            ArgumentNullException.ThrowIfNull(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {key} could not be removed because the connection to the Redis server could not be established", key);

            return Task.CompletedTask;
        }

        logger.LogDebug("The key {key} will be removed from the cache", key);

        return this.redis.Database.KeyDeleteAsync(key);
    }

    /// <summary>
    /// Stores a value in the Redis cache with the provided key.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The key to store the value with.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiration">An optional expiration time for the cached value. If not specified, the default expiration from the options will be used.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The key is <c>null</c> or empty or the value is <c>null</c>.</exception>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(key))
            ArgumentNullException.ThrowIfNull(key);

        if(value == null)
            ArgumentNullException.ThrowIfNull(value);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {key} could not be stored because the connection to the Redis server could not be established", key);

            return Task.CompletedTask;
        }

        if (expiration == null)
            expiration = this.cacheOptions.Expiration;

        logger.LogDebug("The key {key} will be stored in the cache for {expiration} seconds", key, expiration.Value.TotalSeconds);

        return this.redis.Database.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);
    }

}
