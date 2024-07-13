using CodeDesignPlus.Net.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.Redis.Services;

/// <summary>
/// Factory for creating and initializing instances of <see cref="IRedisService"/> based on provided configuration.
/// </summary>
public class RedisServiceFactory : IRedisServiceFactory
{
    /// <summary>
    /// Provides an instance of a registered service.
    /// </summary>
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Configuration options for Redis.
    /// </summary>
    private readonly RedisOptions options;
    private readonly ILogger<RedisServiceFactory> logger;

    /// <summary>
    /// Instances of <see cref="IRedisService"/> that have been created and initialized.
    /// </summary>
    private readonly ConcurrentDictionary<string, IRedisService> instances = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServiceFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to retrieve service instances.</param>
    /// <param name="options">The options configuration for Redis.</param>
    /// <param name="logger">The logger to write diagnostic information.</param>
    public RedisServiceFactory(IServiceProvider serviceProvider, IOptions<RedisOptions> options, ILogger<RedisServiceFactory> logger)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        this.serviceProvider = serviceProvider;
        this.options = options.Value;
        this.logger = logger;

        this.logger.LogInformation("RedisServiceFactory has been initialized");
    }

    /// <summary>
    /// Creates and initializes an instance of <see cref="IRedisService"/> based on the provided instance name.
    /// </summary>
    /// <param name="name">The name of the Redis instance to create.</param>
    /// <returns>An initialized <see cref="IRedisService"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the name is null or empty.</exception>
    /// <exception cref="Exceptions.RedisException">Thrown when the provided instance name does not exist in the registered configurations.</exception>
    public IRedisService Create(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (!options.Instances.TryGetValue(name, out Instance value))
            throw new Exceptions.RedisException($"The redis instance with the name {name} has not been registered");

        if (this.instances.TryGetValue(name, out var service))
            return service;

        service = this.serviceProvider.GetRequiredService<IRedisService>();

        service.Initialize(value);

        this.instances.TryAdd(name, service);

        this.logger.LogInformation("Redis instance {name} has been added to the factory", name);

        return service;
    }
}
