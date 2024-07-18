using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Memory
{
    public class MemoryService : IMemoryService
    {
        public List<UserCreatedEvent> UserEventTrace { get; private set; } = new List<UserCreatedEvent>();
    }
}
