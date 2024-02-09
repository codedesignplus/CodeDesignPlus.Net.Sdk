using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events;


[Key("product-topic")]
public class ProductCreatedEvent : DomainEvent
{
    public ProductCreatedEvent(Guid aggregateId, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object> metadata = null) 
        : base(aggregateId, eventId, occurredAt, metadata)
    {
    }

    public required string Name { get; set; }
}
