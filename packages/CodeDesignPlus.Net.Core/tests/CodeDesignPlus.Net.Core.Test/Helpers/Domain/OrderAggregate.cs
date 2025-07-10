namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;

public class OrderAggregate : AggregateRoot
{
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

    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }


    public static OrderAggregate Create(Guid id, string name, string description, decimal price, Guid tenant, Guid createBy)
    {
        var aggregate = new OrderAggregate(id, name, description, price)
        {
            IsActive = true,
            Tenant = tenant
        };

        aggregate.CreatedAt = SystemClock.Instance.GetCurrentInstant();
        aggregate.CreatedBy = createBy;

        aggregate.AddEvent(new OrderCreatedDomainEvent(id, name, description, price, SystemClock.Instance.GetCurrentInstant(), createBy, null, metadata: new Dictionary<string, object>
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
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        AddEvent(new OrderUpdatedDomainEvent(Id, name, description, price, UpdatedAt, updatedBy));
    }

    public void Delete(Guid deletedBy)
    {
        DeletedAt = SystemClock.Instance.GetCurrentInstant();
        DeletedBy = deletedBy;
        IsDeleted = true;

        AddEvent(new OrderDeletedDomainEvent(Id, DeletedAt));
    }
}
