using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Event.Sourcing.Sample.Aggregates;

namespace CodeDesignPlus.Net.Event.Sourcing.Sample.Events;

[EventKey<OrderAggregate>(1, "updated")]
public class NameUpdatedDomainEvent(
    Guid aggregateId,
    string name,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;
}
