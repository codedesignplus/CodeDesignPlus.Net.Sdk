using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Sample.Resources.DomainEvents;

namespace CodeDesignPlus.Net.Core.Sample.Resources.Aggregate;

public class OrderAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }

    public OrderAggregate() : base()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public OrderAggregate(Guid id, string name, string description, decimal price) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
    }

    public static OrderAggregate Create(Guid id, string name, string description, decimal price, Guid tenant, Guid createBy)
    {
        var aggregate = new OrderAggregate(id, name, description, price)
        {
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            CreatedBy = createBy,
            IsActive = true,
            Tenant = tenant
        };

        aggregate.AddEvent(new OrderCreatedDomainEvent(id, name, description, price, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), createBy, null, metadata: new Dictionary<string, object>
        {
            { "MetaKey1", "Value1" }
        }));

        return aggregate;
    }

    public void Update(string name, string description, decimal price, Guid updatedBy)
    {
        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        UpdatedBy = updatedBy;

        AddEvent(new OrderUpdatedDomainEvent(Id, name, description, price, UpdatedAt, updatedBy));
    }

    public void Delete()
    {
        UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        AddEvent(new OrderDeletedDomainEvent(Id, UpdatedAt));
    }
}