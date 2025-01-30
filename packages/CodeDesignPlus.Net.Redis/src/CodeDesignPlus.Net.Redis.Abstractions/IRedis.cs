namespace CodeDesignPlus.Net.Redis.Abstractions;

/// <summary>
/// Manages the connection to the Redis cluster.
/// </summary>
public interface IRedis
{
    /// <summary>
    /// Gets the Redis connection multiplexer.
    /// </summary>
    IConnectionMultiplexer Connection { get; }

    /// <summary>
    /// Gets the Redis database.
    /// </summary>
    IDatabaseAsync Database { get; }

    /// <summary>
    /// Gets the Redis subscriber for pub/sub scenarios.
    /// </summary>
    ISubscriber Subscriber { get; }

    /// <summary>
    /// Gets a value indicating whether any servers are connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Initiates a connection to the provided Redis instance.
    /// </summary>
    /// <param name="instance">Configuration details of the Redis instance to connect to.</param>
    void Initialize(Instance instance);
}