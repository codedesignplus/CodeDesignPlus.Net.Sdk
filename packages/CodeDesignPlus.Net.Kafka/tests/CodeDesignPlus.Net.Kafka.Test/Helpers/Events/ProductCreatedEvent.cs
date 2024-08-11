using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events;


[EventKey<ProductEntity>(1, "created")]
public class ProductCreatedEvent(Guid aggregateId, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object> metadata = null!)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public required string Name { get; set; }
}
