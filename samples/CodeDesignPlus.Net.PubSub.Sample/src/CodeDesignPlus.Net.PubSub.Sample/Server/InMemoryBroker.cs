using System;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.PubSub.Sample.Server;

public class InMemoryBroker(IServiceProvider serviceProvider, ILogger<InMemoryBroker> logger) : IMessage
{
    private Action<object>? onMessage = null;

    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing event {@event}", @event);

        onMessage?.Invoke(@event);

        return Task.CompletedTask;
    }

    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        foreach (var item in @event)
        {
            logger.LogInformation("Publishing event {@event}", item);

            onMessage?.Invoke(@event);
        }

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        logger.LogInformation("Subscribing to event {event} with handler {handler}", typeof(TEvent).Name, typeof(TEventHandler).Name);

        onMessage = (message) =>
        {
            if (message is TEvent @event)
            {
                var handler = serviceProvider.GetRequiredService<TEventHandler>();

                handler.HandleAsync(@event, cancellationToken);
            }
        };

        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        logger.LogInformation("Unsubscribing to event {event} with handler {handler}", typeof(TEvent).Name, typeof(TEventHandler).Name);

        onMessage = null;

        return Task.CompletedTask;
    }
}
