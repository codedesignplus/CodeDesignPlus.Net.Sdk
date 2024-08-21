namespace CodeDesignPlus.Net.Redis.Services;

/// <summary>
/// Factory for creating and managing Redis services.
/// </summary>
public class RedisServiceFactory : IRedisServiceFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly RedisOptions options;
    private readonly ILogger<RedisServiceFactory> logger;
    private readonly ConcurrentDictionary<string, IRedisService> instances = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServiceFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="options">The Redis options.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/>, <paramref name="options"/>, or <paramref name="logger"/> is null.</exception>
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
    /// Creates a Redis service for the specified instance name.
    /// </summary>
    /// <param name="name">The name of the Redis instance.</param>
    /// <returns>The Redis service.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="Exceptions.RedisException">Thrown when the Redis instance with the specified name is not registered.</exception>
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

        this.logger.LogInformation("Redis instance {Name} has been added to the factory", name);

        return service;
    }
}