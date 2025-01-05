using CodeDesignPlus.Net.EventStore.PubSub.Sample.Events;
using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.EventStore.PubSub.Sample.Consumers;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedDomainEvent>
{
    public Task HandleAsync(OrderCreatedDomainEvent data, CancellationToken token)
    {
        Console.WriteLine($"Order created with Id {data.AggregateId}");

        return Task.CompletedTask;
    }
}
