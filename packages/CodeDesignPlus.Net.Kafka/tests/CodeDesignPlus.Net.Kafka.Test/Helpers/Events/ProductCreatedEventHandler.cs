using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events;

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
        memory.ProductEventTrace.Add(data);

        logger.LogDebug("Invoked Event: {Json}", JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }
}
