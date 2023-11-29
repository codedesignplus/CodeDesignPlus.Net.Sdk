using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Memory;
using Newtonsoft.Json;

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

        this.logger.LogDebug("Invoked Event: {0}", JsonConvert.SerializeObject(data));

        return Task.CompletedTask;
    }
}
