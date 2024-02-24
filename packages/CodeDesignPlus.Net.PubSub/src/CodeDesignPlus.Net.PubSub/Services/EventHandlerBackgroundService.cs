using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.PubSub;

/// <summary>
/// Provides a background service for handling events with a specified event handler.
/// </summary>
/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class EventHandlerBackgroundService<TEventHandler, TEvent> : BackgroundService
    where TEventHandler : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    private readonly ILogger<EventHandlerBackgroundService<TEventHandler, TEvent>> logger;
    private readonly IMessage message;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerBackgroundService{TEventHandler, TEvent}"/> class.
    /// </summary>
    /// <param name="messages">Service for managing events.</param>
    /// <param name="logger">Service for logging.</param>
    public EventHandlerBackgroundService(IEnumerable<IMessage> messages, ILogger<EventHandlerBackgroundService<TEventHandler, TEvent>> logger)
    {
        if(!messages.Any())
            throw new ArgumentException("There is no service available to process events. Please enable an event listener in your service.");

        if (messages.Where(x => x.ListenerEvents).Count() > 1)
            throw new ArgumentException("There are multiple services available to process events. Please disable one of the event listeners in your service.");

        this.message = messages.FirstOrDefault(x => x.ListenerEvents);
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

        Task.Run(() => this.message.SubscribeAsync<TEvent, TEventHandler>(stoppingToken), stoppingToken);

        return Task.CompletedTask;
    }
}
