
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Test;

[Key("order.updated")]
public class OrderUpdatedDomainEvent(
    Guid id,
    string name,
    string description,
    decimal price,
    DateTime? updatedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public DateTime? UpdatedAt { get; private set; } = updatedAt;
}