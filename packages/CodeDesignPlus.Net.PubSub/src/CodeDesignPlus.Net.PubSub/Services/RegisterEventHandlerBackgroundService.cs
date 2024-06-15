using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.PubSub;

/// <summary>
/// Provides a background service for handling events with a specified event handler.
/// </summary>
/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class RegisterEventHandlerBackgroundService<TEventHandler, TEvent> : BackgroundService
    where TEventHandler : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    private readonly ILogger<RegisterEventHandlerBackgroundService<TEventHandler, TEvent>> logger;
    private readonly IMessage message;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterEventHandlerBackgroundService{TEventHandler, TEvent}"/> class.
    /// </summary>
    /// <param name="message">Service for managing events.</param>
    /// <param name="logger">Service for logging.</param>
    public RegisterEventHandlerBackgroundService(IMessage message, ILogger<RegisterEventHandlerBackgroundService<TEventHandler, TEvent>> logger)
    {
        this.message = message ?? throw new ArgumentNullException(nameof(message));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.logger.LogInformation("EventHandlerBackgroundService for EventHandler: {TEventHandler} and Event: {TEvent} has been initialized.", typeof(TEventHandler).Name, typeof(TEvent).Name);
    }

    /// <summary>
    /// Executes the background service task.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the host is performing a graceful shutdown.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Starting execution of {TEventHandler} for event type {TEvent}.", typeof(TEventHandler).Name, typeof(TEvent).Name);

        Task.Run(() => this.message.SubscribeAsync<TEvent, TEventHandler>(stoppingToken).ConfigureAwait(false), stoppingToken);

        return Task.CompletedTask;
    }
}
