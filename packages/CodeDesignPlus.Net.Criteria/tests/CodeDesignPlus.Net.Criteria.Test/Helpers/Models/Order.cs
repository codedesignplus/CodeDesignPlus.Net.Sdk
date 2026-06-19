namespace CodeDesignPlus.Net.Criteria.Test.Helpers.Models;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Cancelled = 3,
}

public class Order
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public Guid Code { get; set; }

    public Instant CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public List<Product> Products { get; set; } = [];

    public Client? Client { get; set; }
}
