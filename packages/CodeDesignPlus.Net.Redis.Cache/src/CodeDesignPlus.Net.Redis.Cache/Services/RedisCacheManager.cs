using CodeDesignPlus.Net.Core.Abstractions.Options;
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
/// <param name="cacheOptions">The options for configuring the Redis cache.</param>
/// <param name="coreOptions">The options for configuring the core of the application.</param>
public class RedisCacheManager(IRedisFactory factory, ILogger<RedisCacheManager> logger, IOptions<RedisCacheOptions> cacheOptions, IOptions<CoreOptions> coreOptions) : IRedisCacheManager
{
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

        return WriteAsync("FLUSHDB", () => this.redis.Database.ExecuteAsync("FLUSHDB"));
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

        var internalKey = GetKey(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {InternalKey} could not be verified because the connection to the Redis server could not be established", internalKey);

            return Task.FromResult(false);
        }

        return ReadAsync(internalKey, () => this.redis.Database.KeyExistsAsync(internalKey), false);
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
            
        var internalKey = GetKey(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {InternalKey} could not be retrieved because the connection to the Redis server could not be established", internalKey);

            return default;
        }

        // Solo la E/S va envuelta: un fallo al deserializar no es indisponibilidad, es un defecto,
        // y debe seguir propagandose.
        var data = await ReadAsync(internalKey, () => this.redis.Database.StringGetAsync(internalKey), StackExchange.Redis.RedisValue.Null);

        if (data.IsNullOrEmpty)
        {
            logger.LogDebug("The key {InternalKey} does not exist in the cache", internalKey);

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
            
        var internalKey = GetKey(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {InternalKey} could not be removed because the connection to the Redis server could not be established", internalKey);

            return Task.CompletedTask;
        }

        logger.LogDebug("The key {InternalKey} will be removed from the cache", internalKey);

        return WriteAsync(internalKey, () => this.redis.Database.KeyDeleteAsync(internalKey));
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

        if (EqualityComparer<T>.Default.Equals(value, default))
            ArgumentNullException.ThrowIfNull(value);

        var internalKey = GetKey(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The key {InternalKey} could not be stored because the connection to the Redis server could not be established", internalKey);

            return Task.CompletedTask;
        }

        if (expiration == null)
            expiration = cacheOptions.Value.Expiration;

        logger.LogDebug("The key {InternalKey} will be stored in the cache for {Expiration_Value_TotalSeconds} seconds", internalKey, expiration.Value.TotalSeconds);

        return WriteAsync(internalKey, () => this.redis.Database.StringSetAsync(internalKey, JsonSerializer.Serialize(value), expiration));
    }

    /// <inheritdoc/>
    public async Task<T> GetGlobalAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The global key {Key} could not be retrieved because the connection to the Redis server could not be established", key);

            return default;
        }

        var data = await ReadAsync(key, () => this.redis.Database.StringGetAsync(key), StackExchange.Redis.RedisValue.Null);

        if (data.IsNullOrEmpty)
        {
            logger.LogDebug("The global key {Key} does not exist in the cache", key);

            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    /// <inheritdoc/>
    public Task SetGlobalAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The global key {Key} could not be stored because the connection to the Redis server could not be established", key);

            return Task.CompletedTask;
        }

        expiration ??= cacheOptions.Value.Expiration;

        logger.LogDebug("The global key {Key} will be stored in the cache for {Seconds} seconds", key, expiration.Value.TotalSeconds);

        return WriteAsync(key, () => this.redis.Database.StringSetAsync(key, JsonSerializer.Serialize(value), expiration));
    }

    /// <inheritdoc/>
    public Task RemoveGlobalAsync(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The global key {Key} could not be removed because the connection to the Redis server could not be established", key);

            return Task.CompletedTask;
        }

        logger.LogDebug("The global key {Key} will be removed from the cache", key);

        return WriteAsync(key, () => this.redis.Database.KeyDeleteAsync(key));
    }

    /// <inheritdoc/>
    public Task AddToGlobalSetAsync(string key, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentException.ThrowIfNullOrEmpty(value);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The member could not be added to the global set {Key} because the connection to the Redis server could not be established", key);

            return Task.CompletedTask;
        }

        return WriteAsync(key, () => this.redis.Database.SetAddAsync(key, value));
    }

    /// <inheritdoc/>
    public Task RemoveFromGlobalSetAsync(string key, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentException.ThrowIfNullOrEmpty(value);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The member could not be removed from the global set {Key} because the connection to the Redis server could not be established", key);

            return Task.CompletedTask;
        }

        return WriteAsync(key, () => this.redis.Database.SetRemoveAsync(key, value));
    }

    /// <inheritdoc/>
    public async Task<string[]> GetGlobalSetMembersAsync(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        if (this.redis.Database == null)
        {
            logger.LogWarning("The global set {Key} could not be read because the connection to the Redis server could not be established", key);

            return [];
        }

        var members = await ReadAsync(key, () => this.redis.Database.SetMembersAsync(key), []);

        return [.. members.Select(member => member.ToString())];
    }

    /// <summary>
    /// Builds the key namespaced by business and application name.
    /// </summary>
    /// <param name="key">The key of the value.</param>
    /// <returns>The namespaced key.</returns>
    private string GetKey(string key)
    {
        return $"{coreOptions.Value.Business}:{coreOptions.Value.AppName}:{key}";
    }

    /// <summary>
    /// Tells apart "the cache cannot answer right now" from "there is a bug".
    /// </summary>
    /// <remarks>
    /// <para>
    /// A cache that takes its caller down with it is not a cache. Only the failures listed here are
    /// swallowed: the connection dropping, the server timing out, the multiplexer being disposed on
    /// shutdown. Anything else —a serialization error, for instance— still propagates, because that
    /// is a defect and hiding it would be worse than failing.
    /// </para>
    /// <para>
    /// The <c>Database == null</c> guard each method already had only covers a connection that was
    /// never established. It does nothing for a server that goes away later, which is what actually
    /// happened: with Redis unreachable, ms-tenants-grpc could not serve a tenant it had sitting in
    /// MongoDB, because the cache probe threw before the query ever ran.
    /// </para>
    /// </remarks>
    /// <param name="exception">The exception raised by the Redis client.</param>
    /// <returns><c>true</c> when the cache is merely unavailable.</returns>
    private static bool IsUnavailable(Exception exception) =>
        exception is StackExchange.Redis.RedisException or TimeoutException or ObjectDisposedException;

    /// <summary>
    /// Runs a read against Redis, degrading to a miss when the cache is unavailable.
    /// </summary>
    private async Task<T> ReadAsync<T>(string key, Func<Task<T>> operation, T onMiss)
    {
        try
        {
            return await operation();
        }
        catch (Exception exception) when (IsUnavailable(exception))
        {
            logger.LogError(exception, "The key {Key} could not be read because the cache is unavailable; continuing as a miss", key);

            return onMiss;
        }
    }

    /// <summary>
    /// Runs a write against Redis, giving up quietly when the cache is unavailable.
    /// </summary>
    /// <remarks>
    /// What is stored here is derived state: losing a write costs a recomputation, never
    /// correctness. It is logged at error level on purpose — silent is not the same as invisible,
    /// and a burst of these is worth an alert.
    /// </remarks>
    private async Task WriteAsync(string key, Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (Exception exception) when (IsUnavailable(exception))
        {
            logger.LogError(exception, "The key {Key} could not be written because the cache is unavailable; the value will be recomputed", key);
        }
    }
}
