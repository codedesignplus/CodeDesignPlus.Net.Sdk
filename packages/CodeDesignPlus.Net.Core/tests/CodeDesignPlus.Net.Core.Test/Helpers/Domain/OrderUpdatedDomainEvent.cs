
namespace CodeDesignPlus.Net.Core.Test;

public class OrderUpdatedDomainEvent(
    Guid id,
    string name,
    string description,
    decimal price,
    DateTime? updatedAt
) : DomainEvent(Guid.NewGuid(), "OrderUpdated", DateTime.UtcNow, id)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public DateTime? UpdatedAt { get; private set; } = updatedAt;
}
