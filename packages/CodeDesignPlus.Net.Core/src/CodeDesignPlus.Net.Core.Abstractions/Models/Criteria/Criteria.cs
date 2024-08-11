namespace CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

/// <summary>
/// Represents the criteria to filter and order the data.
/// </summary>
public class Criteria
{
    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public string? Filters { get; set; }
    /// <summary>
    /// Gets or sets the field to order.
    /// </summary>
    public string? OrderBy { get; set; }
    /// <summary>
    /// Gets or sets the order type.
    /// </summary>
    public OrderTypes OrderType { get; set; }
}
