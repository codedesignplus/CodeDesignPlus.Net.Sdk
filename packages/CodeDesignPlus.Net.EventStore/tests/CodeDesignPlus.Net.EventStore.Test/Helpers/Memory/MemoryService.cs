using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Memory;

public class MemoryService : IMemoryService
{
    public List<OrderCreatedEvent> OrderCreatedEvent { get; private set; } = new();

    public List<ProductAddedToOrderEvent> ProductAddedToOrderEvent { get; private set; } = new();
}

public interface IMemoryService
{
    List<OrderCreatedEvent> OrderCreatedEvent { get; }

    List<ProductAddedToOrderEvent> ProductAddedToOrderEvent { get; }
}