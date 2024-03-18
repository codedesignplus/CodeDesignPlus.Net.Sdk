using CodeDesignPlus.Net.Redis.Diagnostics.Options;

namespace CodeDesignPlus.Net.Redis.Diagnostics.Services;

/// <summary>
/// Default implementation of the <see cref="IRedis.DiagnosticsService"/>
/// </summary>
public class RedisDiagnosticsService : IRedisDiagnosticsService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<RedisDiagnosticsService> logger;
    /// <summary>
    /// Redis.Diagnostics Options
    /// </summary>
    private readonly RedisDiagnosticsOptions options;

    /// <summary>
    /// Initialize a new instance of the <see cref="Redis.DiagnosticsService"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Redis.Diagnostics Options</param>
    public RedisDiagnosticsService(ILogger<RedisDiagnosticsService> logger, IOptions<RedisDiagnosticsOptions> options)
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
