using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events;

public class DummyEventHandler : IEventHandler<DummyEventWithoutTopic>
{
    public Task HandleAsync(DummyEventWithoutTopic data, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
