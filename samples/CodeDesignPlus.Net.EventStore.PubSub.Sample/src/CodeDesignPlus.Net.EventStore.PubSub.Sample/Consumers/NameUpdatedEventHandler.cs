using CodeDesignPlus.Net.EventStore.PubSub.Sample.Events;
using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.EventStore.PubSub.Sample.Consumers;

public class NameUpdatedEventHandler : IEventHandler<NameUpdatedDomainEvent>
{
    public Task HandleAsync(NameUpdatedDomainEvent data, CancellationToken token)
    {
        Console.WriteLine($"Name updated to {data.Name}");

        return Task.CompletedTask;
    }
}
