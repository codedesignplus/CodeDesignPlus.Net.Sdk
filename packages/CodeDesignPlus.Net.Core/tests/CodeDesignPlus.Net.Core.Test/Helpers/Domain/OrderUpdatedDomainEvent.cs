namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;

[EventKey<OrderAggregate>(1, "updated")]
public class OrderUpdatedDomainEvent(
    Guid id,
    string name,
    string description,
    decimal price,
    Instant? updatedAt,
    Guid UpdatedBy,
    Guid? eventId = null,
    Instant? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    private readonly Guid updatedBy = UpdatedBy;

    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public Instant? UpdatedAt { get; private set; } = updatedAt;
}