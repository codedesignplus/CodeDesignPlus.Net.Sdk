
using System.Text.Json.Serialization;

namespace CodeDesignPlus.Net.Core.Test;

public class OrderCreatedDomainEvent(
    Guid aggregateId,
    string name,
    string description,
    decimal price,
    DateTime createdAt,
    DateTime? updatedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
    ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime? UpdatedAt { get; private set; } = updatedAt;

    public override string GetEventType()
    {
        return "OrderCreated";
    }
}
