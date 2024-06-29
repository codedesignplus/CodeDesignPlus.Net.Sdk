namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;

[EventKey<OrderAggregate>(1, "created")]
public class OrderCreatedDomainEvent(
    Guid aggregateId,
    string name,
    string description,
    decimal price,
    long createdAt,
    Guid createBy,
    long? updatedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
    ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public long CreatedAt { get; private set; } = createdAt;
    public Guid CreateBy { get; private set; } = createBy;
    public long? UpdatedAt { get; private set; } = updatedAt;
}
