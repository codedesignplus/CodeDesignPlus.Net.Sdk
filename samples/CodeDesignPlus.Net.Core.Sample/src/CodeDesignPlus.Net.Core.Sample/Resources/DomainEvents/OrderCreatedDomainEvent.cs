using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Core.Sample.Resources.Aggregate;

namespace CodeDesignPlus.Net.Core.Sample.Resources.DomainEvents;

[EventKey<OrderAggregate>(1, "created")]
public class OrderCreatedDomainEvent(
    Guid aggregateId,
    string name,
    string description,
    decimal price,
    Instant createdAt,
    Guid createBy,
    Instant? updatedAt,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
    ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public Instant CreatedAt { get; private set; } = createdAt;
    public Guid CreateBy { get; private set; } = createBy;
    public Instant? UpdatedAt { get; private set; } = updatedAt;
}
