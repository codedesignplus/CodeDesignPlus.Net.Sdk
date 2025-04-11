using CodeDesignPlus.Net.Core.Abstractions;
using NodaTime;

namespace CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Entities;

public class UserAggregate(Guid id) : AggregateRoot(id)
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public List<ProductEntity> Products { get; private set; } = [];

    private UserAggregate(Guid id, string name, string email, Guid tenant, Guid createdBy)
        : this(id)
    {
        Name = name;
        Email = email;
        Tenant = tenant;
        IsActive = true;
        CreatedBy = createdBy;
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }

    public static UserAggregate Create(Guid id, string name, string email, Guid tenant, Guid createdBy)
    {
        return new UserAggregate(id, name, email, tenant, createdBy);
    }

    public void AddProduct(ProductEntity product)
    {
        Products.Add(product);
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}