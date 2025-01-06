using CodeDesignPlus.Net.EventStore.PubSub.Sample.Events;
using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.EventStore.PubSub.Sample.Consumers;

public class ProductAddedEventHandler : IEventHandler<ProductAddedDomainEvent>
{
    public Task HandleAsync(ProductAddedDomainEvent data, CancellationToken token)
    {
        Console.WriteLine($"Product added with Id {data.AggregateId}");

        return Task.CompletedTask;
    }
}
