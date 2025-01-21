
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.EventStore.PubSub.Consumer.Sample.Aggregates;

namespace CodeDesignPlus.Net.EventStore.PubSub.Consumer.Sample.Events;

[EventKey<OrderAggregate>(1, "created", "sample-eventstore-producer")]
public class OrderCreatedDomainEvent(
    Guid aggregateId,
    string name,
    Guid idUser,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;

    public Guid IdUser { get; set; } = idUser;
}
