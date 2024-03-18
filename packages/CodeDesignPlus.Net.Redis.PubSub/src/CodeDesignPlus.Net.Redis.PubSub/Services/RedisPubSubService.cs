using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Options;
using CodeDesignPlus.Net.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.PubSub.Services;

/// <summary>
/// Default implementation of the <see cref="IRedisPubSubService"/>
/// </summary>
public class RedisPubSubService : IRedisPubSubService
{
    private readonly ILogger<RedisPubSubService> logger;
    private readonly IRedisService redisService;
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly IServiceProvider serviceProvider;
    private readonly PubSubOptions pubSubOptions;
    private readonly RedisPubSubOptions options;

    private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new EventContractResolver([])
    };

    /// <summary>
    /// Initialize a new instance of the <see cref="RedisPubSubService"/>
    /// </summary>
    /// <param name="redisServiceFactory">Service that management connection with Redis Server</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="logger">Service logger</param>
    /// <param name="pubSubOptions">The event bus options</param>
    public RedisPubSubService(
        IRedisServiceFactory redisServiceFactory,
        IServiceProvider serviceProvider,
        ILogger<RedisPubSubService> logger,
        IOptions<RedisPubSubOptions> options,
        IOptions<PubSubOptions> pubSubOptions,
        IDomainEventResolverService domainEventResolverService)
    {
        if (redisServiceFactory == null)
            throw new ArgumentNullException(nameof(redisServiceFactory));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (pubSubOptions == null)
            throw new ArgumentNullException(nameof(pubSubOptions));

        if (domainEventResolverService == null)
            throw new ArgumentNullException(nameof(domainEventResolverService));

        this.options = options.Value;
        this.redisService = redisServiceFactory.Create(FactoryConst.RedisPubSub);

        this.domainEventResolverService = domainEventResolverService;
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.pubSubOptions = pubSubOptions.Value;

        this.logger.LogInformation("RedisPubSubService initialized.");
    }

    /// <summary>
    /// Gets a value indicating whether the service is listening for events.
    /// </summary>
    public bool ListenerEvents => this.options.ListenerEvents;

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="token">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    /// <exception cref="ArgumentNullException">@event is null</exception>
    public Task PublishAsync(IDomainEvent @event, CancellationToken token)
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
        var channel = this.domainEventResolverService.GetKeyDomainEvent(@event.GetType());

        var message = JsonConvert.SerializeObject(@event, this.jsonSerializerSettings);

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
    /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);

        return this.redisService.Subscriber.SubscribeAsync(RedisChannel.Literal(channel), (_, v) => this.ListenerEvent<TEvent, TEventHandler>(v, cancellationToken));
    }

    /// <summary>
    /// The handler to invoke when a message is received on channel.
    /// </summary>
    /// <typeparam name="TEvent">Type Event</typeparam>
    /// <typeparam name="TEventHandler">Type Event Handler</typeparam>
    /// <param name="value">The value received</param>    
    /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
    public void ListenerEvent<TEvent, TEventHandler>(RedisValue value, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var @event = JsonConvert.DeserializeObject<TEvent>(value);

        if (this.pubSubOptions.UseQueue)
        {
            var queue = this.serviceProvider.GetService<IQueueService<TEventHandler, TEvent>>();

            queue.Enqueue(@event);
        }
        else
        {
            var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

            eventHandler.HandleAsync(@event, cancellationToken);
        }
    }

    /// <summary>
    /// Unsubscribe from a specified message channel
    /// </summary>
    /// <typeparam name="TEvent">Type Event</typeparam>
    /// <typeparam name="TEventHandler">Type Event Handler</typeparam>
    /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        this.redisService.Subscriber.Unsubscribe(RedisChannel.Literal(channel));

        this.logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Publish a list of domain events
    /// </summary>
    /// <param name="event">Domains event to publish</param>
    /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }

}
