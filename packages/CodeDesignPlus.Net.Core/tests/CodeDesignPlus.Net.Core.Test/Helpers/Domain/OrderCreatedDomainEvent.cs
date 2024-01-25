
namespace CodeDesignPlus.Net.Core.Test;

public class OrderCreatedDomainEvent(
    Guid id,
    string name,
    string description,
    decimal price,
    DateTime createdAt,
    DateTime? updatedAt = null
) : DomainEvent(Guid.NewGuid(), "OrderCreated", DateTime.UtcNow, id)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime? UpdatedAt { get; private set; } = updatedAt;
}
