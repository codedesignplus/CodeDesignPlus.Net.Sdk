using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributees;
using CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Aggregates;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Events;

[EventKey<OrderAggregateRoot>(1, "updated")]
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
