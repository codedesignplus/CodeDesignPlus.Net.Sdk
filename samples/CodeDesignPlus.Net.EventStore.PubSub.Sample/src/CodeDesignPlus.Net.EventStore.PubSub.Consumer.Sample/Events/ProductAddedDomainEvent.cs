using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.EventStore.PubSub.Consumer.Sample.Aggregates;

namespace CodeDesignPlus.Net.EventStore.PubSub.Consumer.Sample.Events;

[EventKey<OrderAggregate>(1, "product-added", "sample-eventstore-producer")]
public class ProductAddedDomainEvent(
    Guid aggregateId,
    string product,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Product { get; set; } = product;
}
