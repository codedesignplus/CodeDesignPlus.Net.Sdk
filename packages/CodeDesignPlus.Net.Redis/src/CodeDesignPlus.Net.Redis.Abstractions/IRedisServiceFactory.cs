namespace CodeDesignPlus.Net.Redis.Abstractions;

/// <summary>
/// Defines a factory for creating and initializing instances of <see cref="IRedisService"/> based on a provided instance name.
/// </summary>
public interface IRedisServiceFactory
{
    /// <summary>
    /// Creates and initializes an instance of <see cref="IRedisService"/> based on the provided instance name.
    /// </summary>
    /// <param name="name">The name of the Redis instance to create.</param>
    /// <returns>An initialized <see cref="IRedisService"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the name is null or empty.</exception>
    /// <exception cref="Exceptions.RedisException">Thrown when the provided instance name does not exist in the registered configurations.</exception>
    IRedisService Create(string name);
}