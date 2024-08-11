namespace CodeDesignPlus.Net.RabbitMQ.Services;

public class ChannelProvider(IRabbitConnection connection, IDomainEventResolverService domainEventResolver, IOptions<CoreOptions> options) : IChannelProvider
{
    private readonly ConcurrentDictionary<string, ChannelModel> channels = new();
    private readonly ConcurrentDictionary<string, string> exchanges = new();

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

    public string ExchangeDeclare<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        return ExchangeDeclare(domainEvent.GetType());
    }

    public IModel GetChannelPublish(Type domainEventType)
    {
        var key = domainEventType.Name;

        return GetChannel(key);
    }

    public IModel GetChannelPublish<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        return GetChannelPublish(domainEvent.GetType());
    }

    public IModel GetChannelConsumer<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var key = typeof(TEventHandler).Name;

        return GetChannel(key);
    }

    public void SetConsumerTag<TEvent, TEventHandler>(string consumerTag)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var key = typeof(TEventHandler).Name;

        if (channels.TryGetValue(key, out var channel))
            channel.ConsumerTag = consumerTag;
    }

    public string GetConsumerTag<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var key = typeof(TEventHandler).Name;

        if (channels.TryGetValue(key, out var channel))
            return channel.ConsumerTag;

        return default;
    }

    private IModel GetChannel(string key)
    {
        if (channels.TryGetValue(key, out var channel))
            return channel.Channel;

        channel = ChannelModel.Create(key, connection.Connection.CreateModel());

        channels.TryAdd(key, channel);

        return channel.Channel;
    }
}
