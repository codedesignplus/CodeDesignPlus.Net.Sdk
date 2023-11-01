using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.Core.Services;

/// <summary>
/// Default implementation of the <see cref="ICoreService"/>
/// </summary>
public class CoreService : ICoreService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<CoreService> logger;
    /// <summary>
    /// Core Options
    /// </summary>
    private readonly CoreOptions options;

    /// <summary>
    /// Initialize a new instance of the <see cref="CoreService"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Core Options</param>
    public CoreService(ILogger<CoreService> logger, IOptions<CoreOptions> options)
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
        this.logger.LogDebug("{section}, Echo {enable}", options.AppName);

        return Task.FromResult(value);
    }
}
