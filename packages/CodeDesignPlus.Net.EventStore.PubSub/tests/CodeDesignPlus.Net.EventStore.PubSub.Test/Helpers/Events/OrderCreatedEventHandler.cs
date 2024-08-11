using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Memory;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger logger;

    private readonly IMemoryService memory;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger, IMemoryService memory)
    {
        this.logger = logger;
        this.memory = memory;
    }

    public Task HandleAsync(OrderCreatedEvent data, CancellationToken token)
    {
        this.memory.OrderCreatedEvent.Add(data);

        this.logger.LogDebug("Invoked Event: {data}", JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }
}
