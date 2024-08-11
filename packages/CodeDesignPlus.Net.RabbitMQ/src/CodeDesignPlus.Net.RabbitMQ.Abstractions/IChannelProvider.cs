namespace CodeDesignPlus.Net.RabbitMQ.Abstractions;

public interface IChannelProvider
{
    IModel GetChannelPublish(Type domainEventType);
    IModel GetChannelPublish<TDomainEvent>(TDomainEvent domainEvent) 
        where TDomainEvent : IDomainEvent;
    string ExchangeDeclare(Type domainEventType);
    string ExchangeDeclare<TDomainEvent>(TDomainEvent domainEvent) 
        where TDomainEvent : IDomainEvent;
    IModel GetChannelConsumer<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;
    void SetConsumerTag<TEvent, TEventHandler>(string consumerTag)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;
    string GetConsumerTag<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;
}
