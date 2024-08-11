namespace CodeDesignPlus.Net.PubSub.Abstractions;

public interface IMessage
{
    /// <summary>
    /// Publish a domain event
    /// </summary>
    /// <param name="event">Domain event to publish</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Task that represents the asynchronous operation</returns>
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken);
    /// <summary>
    /// Publish a list of domain events
    /// </summary>
    /// <param name="event">Domains event to publish</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Task that represents the asynchronous operation</returns>
    Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken);
    /// <summary>
    /// Subcribe to a domain event
    /// </summary>
    /// <typeparam name="TEvent">The domain event to subscribe</typeparam>
    /// <typeparam name="TEventHandler">The event handler (Callback)</typeparam>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Task that represents the asynchronous operation</returns>
    Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;
    /// <summary>
    /// Unsubscribe to a domain event
    /// </summary>
    /// <typeparam name="TEvent">The domain event to subscribe</typeparam>
    /// <typeparam name="TEventHandler">The event handler (Callback)</typeparam>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Task that represents the asynchronous operation</returns>
    Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>;
}
