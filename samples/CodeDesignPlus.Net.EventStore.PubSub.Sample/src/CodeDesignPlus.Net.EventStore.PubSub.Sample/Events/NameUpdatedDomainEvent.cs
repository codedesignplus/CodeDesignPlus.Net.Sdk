using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.EventStore.PubSub.Sample.Aggregates;

namespace CodeDesignPlus.Net.EventStore.PubSub.Sample.Events;

[EventKey<OrderAggregate>(1, "updated")]
public class NameUpdatedDomainEvent(
    Guid aggregateId,
    string name,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;
}
