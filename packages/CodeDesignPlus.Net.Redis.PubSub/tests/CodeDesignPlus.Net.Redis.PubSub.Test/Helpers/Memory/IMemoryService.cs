using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Memory
{
    public interface IMemoryService
    {
        List<UserCreatedEvent> UserEventTrace { get; }
    }
}
