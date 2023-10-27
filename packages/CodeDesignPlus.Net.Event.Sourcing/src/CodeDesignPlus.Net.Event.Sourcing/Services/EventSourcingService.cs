using CodeDesignPlus.Net.Event.Sourcing.Options;

namespace CodeDesignPlus.Net.Event.Sourcing.Services;

/// <summary>
/// Default implementation of the <see cref="IEvent.SourcingService"/>
/// </summary>
public class EventSourcingService : IEventSourcingService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<EventSourcingService> logger;
    /// <summary>
    /// Event.Sourcing Options
    /// </summary>
    private readonly EventSourcingOptions options;

    /// <summary>
    /// Initialize a new instance of the <see cref="Event.SourcingService"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Event.Sourcing Options</param>
    public EventSourcingService(ILogger<EventSourcingService> logger, IOptions<EventSourcingOptions> options)
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
