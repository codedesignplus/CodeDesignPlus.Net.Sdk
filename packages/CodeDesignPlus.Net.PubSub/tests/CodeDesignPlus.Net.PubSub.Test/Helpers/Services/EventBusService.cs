using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Test;

public class PubSubService : IMessage
{
    public bool ListenerEvents => true;

    public Task PublishAsync(IDomainEvent @event, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task<TResult> PublishAsync<TResult>(IDomainEvent @event, CancellationToken token)
    {
        return Task.FromResult<TResult>(default!);
    }

    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}