using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;


public class OrderProduct
{
    public required Product Product { get; set; }
    public int Quantity { get; set; }
}

public class Client : IEntityBase
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}

public class Product : IEntityBase
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
}