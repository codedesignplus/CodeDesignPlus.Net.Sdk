namespace CodeDesignPlus.Net.RabbitMQ.Abstractions;

/// <summary>
/// Provides methods to manage RabbitMQ channels and exchanges.
/// </summary>
public interface IChannelProvider
{
    /// <summary>
    /// Gets the channel for publishing the specified domain event type.
    /// </summary>
    /// <param name="domainEventType">The type of the domain event.</param>
    /// <returns>The channel for publishing.</returns>
    IModel GetChannelPublish(Type domainEventType);

    /// <summary>
    /// Gets the channel for publishing the specified domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event instance.</param>
    /// <returns>The channel for publishing.</returns>
    IModel GetChannelPublish<TDomainEvent>(TDomainEvent domainEvent) 
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Declares an exchange for the specified domain event type.
    /// </summary>
    /// <param name="domainEventType">The type of the domain event.</param>
    /// <returns>The name of the declared exchange.</returns>
    string ExchangeDeclare(Type domainEventType);

    /// <summary>
    /// Declares an exchange for the specified domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The domain event instance.</param>
    /// <returns>The name of the declared exchange.</returns>
    string ExchangeDeclare<TDomainEvent>(TDomainEvent domainEvent) 
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Gets the channel for consuming the specified domain event with the specified event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <returns>The channel for consuming.</returns>
    IModel GetChannelConsumer<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    /// Sets the consumer tag for the specified event and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="consumerTag">The consumer tag.</param>
    void SetConsumerTag<TEvent, TEventHandler>(string consumerTag)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    /// Gets the consumer tag for the specified event and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <returns>The consumer tag.</returns>
    string GetConsumerTag<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;
}