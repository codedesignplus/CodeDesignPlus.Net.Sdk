using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Kafka.Test;

public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    private readonly ILogger logger;

    private readonly IMemoryService memory;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger, IMemoryService memory)
    {
        this.logger = logger;
        this.memory = memory;
    }

    public Task HandleAsync(ProductCreatedEvent data, CancellationToken token)
    {
        this.memory.ProductEventTrace.Add(data);

        this.logger.LogDebug("Invoked Event: {0}", JsonConvert.SerializeObject(data));

        return Task.CompletedTask;
    }
}
