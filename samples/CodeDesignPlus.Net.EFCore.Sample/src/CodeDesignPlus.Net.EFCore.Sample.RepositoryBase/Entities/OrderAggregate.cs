using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Sample.RepositoryBase.Entities;

public class OrderAggregate : AggregateRoot, IEntity
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
            CreatedAt = SystemClock.Instance.GetCurrentInstant(),
            CreatedBy = createBy,
            IsActive = true,
            Tenant = tenant
        };

        return aggregate;
    }

    public void Update(string name, string description, decimal price, Guid updatedBy)
    {
        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;
    }

    public void Delete(Guid updatedBy)
    {
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;
    }
}
