namespace CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

public class Criteria
{
    public string? Filters { get; set; }

    public Order Order { get; set; } = new();
}
