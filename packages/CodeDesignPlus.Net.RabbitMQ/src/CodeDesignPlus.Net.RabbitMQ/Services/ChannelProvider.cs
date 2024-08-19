namespace CodeDesignPlus.Net.RabbitMQ.Services;

/// <summary>
/// Provides methods to manage RabbitMQ channels.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ChannelProvider"/> class.
/// </remarks>
/// <param name="connection">The RabbitMQ connection.</param>
/// <param name="domainEventResolver">The domain event resolver service.</param>
/// <param name="options">The core options.</param>
public class ChannelProvider(IRabbitConnection connection, IDomainEventResolverService domainEventResolver, IOptions<CoreOptions> options) : IChannelProvider
{
    private readonly IRabbitConnection connection = connection;
    private readonly IDomainEventResolverService domainEventResolver = domainEventResolver;
    private readonly IOptions<CoreOptions> options = options;
    private readonly ConcurrentDictionary<string, ChannelModel> channels = new();
    private readonly ConcurrentDictionary<string, string> exchanges = new();

    /// <summary>
    /// Declares an exchange for the specified domain event type.
    /// </summary>
    /// <param name="domainEventType">The type of the domain event.</param>
    /// <returns>The name of the declared exchange.</returns>
    public string ExchangeDeclare(Type domainEventType)
    {
        var key = domainEventType.Name;

        if (exchanges.TryGetValue(key, out var exchange))
            return exchange;

        var exchangeName = domainEventResolver.GetKeyDomainEvent(domainEventType);

        var channel = GetChannelPublish(domainEventType);

        channel.ExchangeDeclare(exchange: exchangeName, durable: true, type: ExchangeType.Fanout, arguments: new Dictionary<string, object>
        {
            { "x-cdp-bussiness", options.Value.Business },
            { "x-cdp-microservice", options.Value.AppName }
        });

        exchanges.TryAdd(key, exchangeName);

        return exchangeName;
    }

    /// <summary>
    /// Declares an exchange for the specified domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event instance.</param>
    /// <returns>The name of the declared exchange.</returns>
    public string ExchangeDeclare<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        return ExchangeDeclare(domainEvent.GetType());
    }

    /// <summary>
    /// Gets the channel for publishing the specified domain event type.
    /// </summary>
    /// <param name="domainEventType">The type of the domain event.</param>
    /// <returns>The channel for publishing.</returns>
    public IModel GetChannelPublish(Type domainEventType)
    {
        var key = domainEventType.Name;

        return GetChannel(key);
    }

    /// <summary>
    /// Gets the channel for publishing the specified domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event instance.</param>
    /// <returns>The channel for publishing.</returns>
    public IModel GetChannelPublish<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        return GetChannelPublish(domainEvent.GetType());
    }

    /// <summary>
    /// Gets the channel for consuming the specified domain event with the specified event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <returns>The channel for consuming.</returns>
    public IModel GetChannelConsumer<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var key = typeof(TEventHandler).Name;

        return GetChannel(key);
    }

    /// <summary>
    /// Sets the consumer tag for the specified event and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="consumerTag">The consumer tag.</param>
    public void SetConsumerTag<TEvent, TEventHandler>(string consumerTag)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var key = typeof(TEventHandler).Name;

        if (channels.TryGetValue(key, out var channel))
            channel.ConsumerTag = consumerTag;
    }

    /// <summary>
    /// Gets the consumer tag for the specified event and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <returns>The consumer tag.</returns>
    public string GetConsumerTag<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var key = typeof(TEventHandler).Name;

        if (channels.TryGetValue(key, out var channel))
            return channel.ConsumerTag;

        return default;
    }

    /// <summary>
    /// Gets the channel for the specified key.
    /// </summary>
    /// <param name="key">The key for the channel.</param>
    /// <returns>The channel.</returns>
    private IModel GetChannel(string key)
    {
        if (channels.TryGetValue(key, out var channel))
            return channel.Channel;

        channel = ChannelModel.Create(key, connection.Connection.CreateModel());

        channels.TryAdd(key, channel);

        return channel.Channel;
    }
}