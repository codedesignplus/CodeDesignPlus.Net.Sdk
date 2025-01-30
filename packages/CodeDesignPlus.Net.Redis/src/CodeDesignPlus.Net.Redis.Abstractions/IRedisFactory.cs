namespace CodeDesignPlus.Net.Redis.Abstractions;

/// <summary>
/// Defines a factory for creating and initializing instances of <see cref="IRedis"/> based on a provided instance name.
/// </summary>
public interface IRedisFactory
{
    /// <summary>
    /// Creates and initializes an instance of <see cref="IRedis"/> based on the provided instance name.
    /// </summary>
    /// <param name="name">The name of the Redis instance to create.</param>
    /// <returns>An initialized <see cref="IRedis"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the name is null or empty.</exception>
    IRedis Create(string name);
}