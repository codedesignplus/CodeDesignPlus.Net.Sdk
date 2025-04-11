namespace CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

/// <summary>
/// Represents a criteria for filtering and ordering data.
/// </summary>
public class Criteria
{
    /// <summary>
    /// Gets or sets the filters to apply to the data.
    /// </summary>
    public string? Filters { get; set; }

    /// <summary>
    /// Gets or sets the field to order the data by.
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the type of ordering to apply to the data.
    /// </summary>
    public OrderTypes OrderType { get; set; }    
    
    /// <summary>
    /// Gets or sets the number of records to skip.
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of records to return.
    /// </summary>
    public int? Limit { get; set; }
}
