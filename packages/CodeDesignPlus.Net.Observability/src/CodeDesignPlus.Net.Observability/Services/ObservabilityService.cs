using CodeDesignPlus.Net.Observability.Abstractions.Options;

namespace CodeDesignPlus.Net.Observability.Services;

/// <summary>
/// Default implementation of the <see cref="IObservabilityService"/>
/// </summary>
public class ObservabilityService : IObservabilityService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<ObservabilityService> logger;
    /// <summary>
    /// Observability Options
    /// </summary>
    private readonly ObservabilityOptions options;

    /// <summary>
    /// Initialize a new instance of the <see cref="ObservabilityService"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Observability Options</param>
    public ObservabilityService(ILogger<ObservabilityService> logger, IOptions<ObservabilityOptions> options)
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
        this.logger.LogDebug("{section}, Echo {enable}", ObservabilityOptions.Section, options.Enable);

        return Task.FromResult(value);
    }
}
