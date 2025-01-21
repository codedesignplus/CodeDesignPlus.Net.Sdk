using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;

[EventKey<ProductEntity>(1, "created")]
public class ProductCreatedDomainEvent(Guid aggregateId, string? name, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? Name { get; set; } = name;

}
