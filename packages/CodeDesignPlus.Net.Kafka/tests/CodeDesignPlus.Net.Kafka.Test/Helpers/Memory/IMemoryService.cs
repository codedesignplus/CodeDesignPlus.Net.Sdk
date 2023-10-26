using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Memory
{
    public interface IMemoryService
    {
        List<UserCreatedEvent> UserEventTrace { get; }
        List<ProductCreatedEvent> ProductEventTrace { get; }
    }
}
