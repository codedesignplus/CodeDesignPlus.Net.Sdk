using System.Text.Json;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Options;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.PubSub.Services;

/// <summary>
/// Default implementation of the <see cref="IRedisPubSubService"/>
/// </summary>
public class RedisPubSubService : IRedisPubSubService
{
    /// <summary>
    /// Service logger
    /// </summary>
    private readonly ILogger<RedisPubSubService> logger;
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
    private readonly PubSubOptions pubSubOptions;

    /// <summary>
    /// Initialize a new instance of the <see cref="RedisPubSubService"/>
    /// </summary>
    /// <param name="redisServiceFactory">Service that management connection with Redis Server</param>
    /// <param name="subscriptionManager">Service that management the events and events handlers inside assembly</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="logger">Service logger</param>
    /// <param name="PubSubOptions">The event bus options</param>
    public RedisPubSubService(
        IRedisServiceFactory redisServiceFactory,
        ISubscriptionManager subscriptionManager,
        IServiceProvider serviceProvider,
        ILogger<RedisPubSubService> logger,
        IOptions<RedisPubSubOptions> options,
        IOptions<PubSubOptions> PubSubOptions)
    {
        if (redisServiceFactory == null)
            throw new ArgumentNullException(nameof(redisServiceFactory));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (PubSubOptions == null)
            throw new ArgumentNullException(nameof(PubSubOptions));

        this.redisService = redisServiceFactory.Create(options.Value.Name);

        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.pubSubOptions = PubSubOptions.Value;

        this.logger.LogInformation("RedisPubSubService initialized.");
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

        this.logger.LogInformation("Publishing event: {TEvent}.", @event.GetType().Name);

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

        this.logger.LogInformation("Event {TEvent} published with {notified} notifications.", @event.GetType().Name, notified);

        return (TResult)Convert.ChangeType(notified, typeof(TResult));
    }

    /// <summary>
    /// This method is invoked when register the subscribe with <see cref="CodeDesignPlus.PubSub.Extensions.PubSubExtensions.SubscribeEventsHandlers{TStartupLogic}(IServiceProvider)"/> extension method
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

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);

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
        if (this.subscriptionManager.HasSubscriptionsForEvent<TEvent>())
        {
            this.logger.LogInformation("Received event: {TEvent}.", typeof(TEvent).Name);

            var subscriptions = this.subscriptionManager.FindSubscriptions<TEvent>();

            foreach (var subscription in subscriptions)
            {
                var @event = JsonSerializer.Deserialize<TEvent>(value);

                if (this.pubSubOptions.EnableQueue)
                {
                    var queue = this.serviceProvider.GetService<IQueueService<TEventHandler, TEvent>>();

                    queue.Enqueue(@event);
                }
                else
                {
                    var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

                    eventHandler.HandleAsync(@event, token);
                }
            }
        }
        else
        {
            this.logger.LogWarning("No subscriptions found for event: {TEvent}.", typeof(TEvent).Name);
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

        this.subscriptionManager.RemoveSubscription<TEvent, TEventHandler>();

        this.redisService.Subscriber.Unsubscribe(RedisChannel.Literal(channel));

        this.logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);
    }
}
