
namespace CodeDesignPlus.Net.Core.Test;

public class OrderAggregate(Guid id, string name, string description, decimal price)
    : AggregateRoot(id)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;

    public static OrderAggregate Create(Guid id, string name, string description, decimal price, Guid createBy)
    {
        var aggregate = new OrderAggregate(id, name, description, price)
        {
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            CreatedBy = createBy
        };

        aggregate.AddEvent(new OrderCreatedDomainEvent(id, name, description, price,DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), createBy, null, metadata: new Dictionary<string, object>
        {
            { "MetaKey1", "Value1" }
        }));

        return aggregate;
    }

    public void Update(string name, string description, decimal price, Guid updatedBy)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        this.UpdatedBy = updatedBy;

        this.AddEvent(new OrderUpdatedDomainEvent(this.Id, name, description, price, this.UpdatedAt, updatedBy));
    }

    public void Delete()
    {
        this.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        this.AddEvent(new OrderDeletedDomainEvent(this.Id, this.UpdatedAt));
    }
}
