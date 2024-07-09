using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers.Models;

public class Order : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Total { get; set; }
    public long CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public Client? Client { get; set; }
    public List<Product> Products { get; set; } = [];
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid Tenant { get; set; }
}
