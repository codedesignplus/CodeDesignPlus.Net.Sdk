using CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Memory
{
    public class MemoryService: IMemoryService
    {
        public List<UserCreatedEvent> UserEventTrace { get; private set; } = new List<UserCreatedEvent>();
    }
}
