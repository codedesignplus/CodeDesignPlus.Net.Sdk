using CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Memory
{
    public interface IMemoryService
    {
        List<UserCreatedEvent> UserEventTrace { get; }
    }
}
