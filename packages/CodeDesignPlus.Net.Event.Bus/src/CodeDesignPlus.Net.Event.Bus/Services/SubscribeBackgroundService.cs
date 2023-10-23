using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CodeDesignPlus.Net.Event.Bus.Extensions;

namespace CodeDesignPlus.Net.Event.Bus;

/// <summary>
/// Represents a background service responsible for subscribing to Kafka topics based on registered event handlers.
/// </summary>
public class SubscribeBackgroundService : BackgroundService
{
    /// <summary>
    /// The manager responsible for handling event subscriptions.
    /// </summary>
    private readonly ISubscriptionManager subscriptionManager;
    /// <summary>
    /// The service provider to retrieve services from the container.
    /// </summary>
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriberBackgroundService{TStartupLogic}"/> class.
    /// </summary>
    /// <param name="subscriptionManager">The manager responsible for handling event subscriptions.</param>
    /// <param name="serviceProvider">The service provider to retrieve services from the container.</param>
    public SubscribeBackgroundService(ISubscriptionManager subscriptionManager, IServiceProvider serviceProvider)
    {
        this.subscriptionManager = subscriptionManager;
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Executes the background service task.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> that can be used to stop the execution.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var typeSubscriptionManager = subscriptionManager.GetType();
        var methodAddSubscription = typeSubscriptionManager.GetMethods().FirstOrDefault(x => x.Name.Contains("AddSubscription"));

        var eventBus = this.serviceProvider.GetRequiredService<IEventBus>();
        var typeEventBus = eventBus.GetType();

        var eventsHandlers = EventBusExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            if (interfaceEventHandlerGeneric == null)
                continue;

            var member = interfaceEventHandlerGeneric.GetGenericArguments().FirstOrDefault(x => x.IsSubclassOf(typeof(EventBase)));

            if (member == null && member.IsGenericParameter)
                continue;

            // Register the event handler with the subscription manager
            var methodAdd = methodAddSubscription.MakeGenericMethod(member, eventHandler);
            methodAdd.Invoke(subscriptionManager, null);

            // Subscribe to the event using the event bus
            var methodSuscribe = typeEventBus.GetMethods().FirstOrDefault(x => x.Name == nameof(IEventBus.SubscribeAsync) && x.IsGenericMethod);
            var methodGeneric = methodSuscribe.MakeGenericMethod(member, eventHandler);

            (methodGeneric.Invoke(eventBus, new object[] { stoppingToken }) as Task).ConfigureAwait(false);
        }

        return Task.CompletedTask;
    }
}