using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

public abstract class DomainEventBase : EventBase, IDomainEvent
{
    public Guid AggregateId { get; private set; }
    public string EventType => GetType().Name;

    public DomainEventBase(Guid aggregateId)
    {
        this.AggregateId = aggregateId;
    }
}