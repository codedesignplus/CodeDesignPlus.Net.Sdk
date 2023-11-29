namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;


public class OrderProduct
{
    public required Product Product { get; set; }
    public int Quantity { get; set; }
}

public class Client
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}

public class Product
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
}