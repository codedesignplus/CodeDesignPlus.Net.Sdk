using CodeDesignPlus.Net.Security.Abstractions.Options;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Default implementation of the <see cref="ISecurityService"/>
/// </summary>
public class SecurityService : ISecurityService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<SecurityService> logger;
    /// <summary>
    /// Security Options
    /// </summary>
    private readonly SecurityOptions options;

    /// <summary>
    /// Initialize a new instance of the <see cref="SecurityService"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Security Options</param>
    public SecurityService(ILogger<SecurityService> logger, IOptions<SecurityOptions> options)
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
        return Task.FromResult(value);
    }
}
