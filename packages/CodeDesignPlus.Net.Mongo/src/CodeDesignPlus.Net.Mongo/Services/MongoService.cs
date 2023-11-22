using CodeDesignPlus.Net.Mongo.Abstractions.Options;

namespace CodeDesignPlus.Net.Mongo.Services;

/// <summary>
/// Default implementation of the <see cref="IMongoService"/>
/// </summary>
public class MongoService : IMongoService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<MongoService> logger;
    /// <summary>
    /// Mongo Options
    /// </summary>
    private readonly MongoOptions options;

    /// <summary>
    /// Initialize a new instance of the <see cref="MongoService"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Mongo Options</param>
    public MongoService(ILogger<MongoService> logger, IOptions<MongoOptions> options)
    {
        this.logger = logger;
        this.options = options.Value;
    }

    /// <summary>
    /// Asynchronously echoes the specified value. 
    /// </summary>
    /// <param name="value">The value to echo.</param>
    /// <returns>A task that represents the asynchronous echo operation. The result of the task is the echoed value as a</returns>
    public Task<string> EchoAsync(string value)
    {
        this.logger.LogDebug("{section}, Echo {enable}", options.Enable);

        return Task.FromResult(value);
    }
}
