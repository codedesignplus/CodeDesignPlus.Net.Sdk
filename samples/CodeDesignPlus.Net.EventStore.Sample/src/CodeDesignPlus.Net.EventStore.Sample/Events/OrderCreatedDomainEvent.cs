
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.EventStore.Sample.Aggregates;

namespace CodeDesignPlus.Net.EventStore.Sample.Events;

[EventKey<OrderAggregate>(1, "created")]
public class OrderCreatedDomainEvent(
    Guid aggregateId,
    string name,
    Guid idUser,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;

    public Guid IdUser { get; set; } = idUser;
}
