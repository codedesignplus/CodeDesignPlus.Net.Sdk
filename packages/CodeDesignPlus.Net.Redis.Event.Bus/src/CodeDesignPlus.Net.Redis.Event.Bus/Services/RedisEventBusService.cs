using System.Text.Json;
using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Options;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Event.Bus.Options;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Services;

/// <summary>
/// Default implementation of the <see cref="IRedisEventBusService"/>
/// </summary>
public class RedisEventBusService : IRedisEventBusService
{
    /// <summary>
    /// Service logger
    /// </summary>
    private readonly ILogger<RedisEventBusService> logger;
    /// <summary>
    /// Service that management connection with Redis Server
    /// </summary>
    private readonly IRedisService redisService;
    /// <summary>
    /// Service that management the events and events handlers inside assembly
    /// </summary>
    private readonly ISubscriptionManager subscriptionManager;
    /// <summary>
    /// Service provider
    /// </summary>
    private readonly IServiceProvider serviceProvider;
    /// <summary>
    /// The event bus options
    /// </summary>
    private readonly EventBusOptions eventBusOptions;

    /// <summary>
    /// Initialize a new instance of the <see cref="RedisEventBusService"/>
    /// </summary>
    /// <param name="redisServiceFactory">Service that management connection with Redis Server</param>
    /// <param name="subscriptionManager">Service that management the events and events handlers inside assembly</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="logger">Service logger</param>
    /// <param name="eventBusOptions">The event bus options</param>
    public RedisEventBusService(
        IRedisServiceFactory redisServiceFactory,
        ISubscriptionManager subscriptionManager,
        IServiceProvider serviceProvider,
        ILogger<RedisEventBusService> logger,
        IOptions<RedisEventBusOptions> options,
        IOptions<EventBusOptions> eventBusOptions)
    {
        if (redisServiceFactory == null)
            throw new ArgumentNullException(nameof(redisServiceFactory));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (eventBusOptions == null)
            throw new ArgumentNullException(nameof(eventBusOptions));

        this.redisService = redisServiceFactory.Create(options.Value.Name);

        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.eventBusOptions = eventBusOptions.Value;
    }

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="token">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    /// <exception cref="ArgumentNullException">@event is null</exception>
    public Task PublishAsync(EventBase @event, CancellationToken token)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        return this.PrivatePublishAsync<long>(@event, token);
    }

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <typeparam name="TResult">Type result (long)</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="token">The cancellation token that will be assigned to the new task.</param>
    /// <returns>The number of clients that received the message.</returns>
    private async Task<TResult> PrivatePublishAsync<TResult>(object @event, CancellationToken token)
    {
        var channel = @event.GetType().Name;

        var message = JsonSerializer.Serialize(@event);

        var notified = await this.redisService.Subscriber.PublishAsync(RedisChannel.Literal(channel), message);

        this.logger.LogDebug($"The number of clients notified {notified} in the channel {channel} with the next message {message}");

        return (TResult)Convert.ChangeType(notified, typeof(TResult));
    }

    /// <summary>
    /// This method is invoked when register the subscribe with <see cref="CodeDesignPlus.Event.Bus.Extensions.EventBusExtensions.SubscribeEventsHandlers{TStartupLogic}(IServiceProvider)"/> extension method
    /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, without any guarantee of ordered handling.
    /// </summary>
    /// <typeparam name="TEvent">Type Event</typeparam>
    /// <typeparam name="TEventHandler">Type Event Handler</typeparam>
    /// <param name="token">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = typeof(TEvent).Name;

        this.logger.LogDebug($"Register client in the channel {channel}");

        return this.redisService.Subscriber.SubscribeAsync(RedisChannel.Literal(channel), (_, v) => this.ListenerEvent<TEvent, TEventHandler>(v, token));
    }

    /// <summary>
    /// The handler to invoke when a message is received on channel.
    /// </summary>
    /// <typeparam name="TEvent">Type Event</typeparam>
    /// <typeparam name="TEventHandler">Type Event Handler</typeparam>
    /// <param name="value">The value received</param>    
    /// <param name="token">The cancellation token that will be assigned to the new task.</param>
    public void ListenerEvent<TEvent, TEventHandler>(RedisValue value, CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogDebug($"Message received on the channel {typeof(TEvent).Name} with message {value}");

        if (this.subscriptionManager.HasSubscriptionsForEvent<TEvent>())
        {
            var subscriptions = this.subscriptionManager.FindSubscriptions<TEvent>();

            foreach (var subscription in subscriptions)
            {
                this.logger.LogDebug($"The message will add to the queue with event {subscription.EventType.Name} and the handler {subscription.EventHandlerType.Name}");

                var @event = JsonSerializer.Deserialize<TEvent>(value);

                if (this.eventBusOptions.EnableQueue)
                {

                    var queueType = typeof(IQueueService<,>);

                    queueType = queueType.MakeGenericType(subscription.EventHandlerType, subscription.EventType);

                    var queue = this.serviceProvider.GetService(queueType);

                    queue.GetType().GetMethod(nameof(IQueueService<TEventHandler, TEvent>.Enqueue)).Invoke(queue, new object[] { @event });
                }
                else
                {
                    var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

                    eventHandler.HandleAsync(@event, token);
                }

                this.logger.LogDebug($"The message was added successfully");
            }
        }
    }

    /// <summary>
    /// Unsubscribe from a specified message channel
    /// </summary>
    /// <typeparam name="TEvent">Type Event</typeparam>
    /// <typeparam name="TEventHandler">Type Event Handler</typeparam>
    public void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = typeof(TEvent).Name;

        this.logger.LogDebug($"Remove subscription of the channel {channel}");

        this.subscriptionManager.RemoveSubscription<TEvent, TEventHandler>();

        this.redisService.Subscriber.Unsubscribe(RedisChannel.Literal(channel));
    }
}
