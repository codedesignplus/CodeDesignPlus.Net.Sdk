using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Memory;

public class MemoryService : IMemoryService
{
    public List<OrderCreatedEvent> OrderCreatedEvent { get; private set; } = [];

    public List<ProductAddedToOrderEvent> ProductAddedToOrderEvent { get; private set; } = [];
}

public interface IMemoryService
{
    List<OrderCreatedEvent> OrderCreatedEvent { get; }

    List<ProductAddedToOrderEvent> ProductAddedToOrderEvent { get; }
}