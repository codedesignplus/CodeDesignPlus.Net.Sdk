using CodeDesignPlus.Net.Redis.Options;
using Microsoft.Extensions.DependencyInjection;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServiceFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to retrieve service instances.</param>
    /// <param name="options">The options configuration for Redis.</param>
    public RedisServiceFactory(IServiceProvider serviceProvider, IOptions<RedisOptions> options)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
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

        if (!this.options.Instances.ContainsKey(name))
            throw new Exceptions.RedisException($"The redis instance with the name {name} has not been registered");

        var service = this.serviceProvider.GetRequiredService<IRedisService>();

        service.Initialize(options.Instances[name]);

        return service;
    }
}
