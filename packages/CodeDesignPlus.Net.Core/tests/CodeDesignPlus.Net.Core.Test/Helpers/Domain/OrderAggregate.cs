
namespace CodeDesignPlus.Net.Core.Test;

public class OrderAggregate(Guid id, string name, string description, decimal price, DateTime createdAt,  DateTime? updatedAt = null)
    : AggregateRoot(id)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime? UpdatedAt { get; private set; } = updatedAt;

    public static OrderAggregate Create(Guid id, string name, string description, decimal price)
    {
        var aggregate = new OrderAggregate(id, name, description, price, DateTime.UtcNow, null);

        aggregate.AddEvent(new OrderCreatedDomainEvent(id, name, description, price, DateTime.UtcNow, null, metadata: new Dictionary<string, object>
        {
            { "MetaKey1", "Value1" }
        }));

        return aggregate;
    }

    public void Update(string name, string description, decimal price, DateTime updatedAt)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.UpdatedAt = updatedAt;

        this.AddEvent(new OrderUpdatedDomainEvent(this.Id, name, description, price, updatedAt));
    }

    public void Delete(DateTime deletedAt)
    {
        this.UpdatedAt = deletedAt;

        this.AddEvent(new OrderDeletedDomainEvent(this.Id, deletedAt));
    }
}
