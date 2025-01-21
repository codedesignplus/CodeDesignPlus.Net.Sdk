namespace CodeDesignPlus.Net.Criteria.Sample.Models;

public class Order
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Total { get; set; }

    public Instant CreatedAt { get; set; }

    public List<Product> Products { get; set; } = [];

    public Client? Client { get; set; }
}
