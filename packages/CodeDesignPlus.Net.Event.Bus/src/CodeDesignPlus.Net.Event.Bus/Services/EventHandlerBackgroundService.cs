using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Event.Bus;

/// <summary>
/// Provides a background service for handling events with a specified event handler.
/// </summary>
/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class EventHandlerBackgroundService<TEventHandler, TEvent> : BackgroundService
    where TEventHandler : IEventHandler<TEvent>
    where TEvent : EventBase
{
    private readonly ILogger<EventHandlerBackgroundService<TEventHandler, TEvent>> logger;
    private readonly ISubscriptionManager subscriptionManager;
    private readonly IEventBus eventBus;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerBackgroundService{TEventHandler, TEvent}"/> class.
    /// </summary>
    /// <param name="subscriptionManager">Manages the event subscriptions.</param>
    /// <param name="eventBus">Service for managing events.</param>
    /// <param name="logger">Service for logging.</param>
    public EventHandlerBackgroundService(ISubscriptionManager subscriptionManager, IEventBus eventBus, ILogger<EventHandlerBackgroundService<TEventHandler, TEvent>> logger)
    {
        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
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

        this.subscriptionManager.AddSubscription<TEvent, TEventHandler>();

        this.logger.LogInformation("Subscription added for {TEventHandler} and event type {TEvent}.", typeof(TEventHandler).Name, typeof(TEvent).Name);

        this.eventBus.SubscribeAsync<TEvent, TEventHandler>(stoppingToken);

        return Task.CompletedTask;
    }
}
