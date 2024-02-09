using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Events;

[Key("product.added")]
public class ProductAddedDomainEvent(
    Guid aggregateId, 
    string product,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Product { get; set; } = product;
}
