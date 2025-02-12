namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents the country information.
/// </summary>
public class Country
{
    /// <summary>
    /// Gets or sets the identifier of the country.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the country.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the code of the country.
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Gets or sets the time zone of the country.
    /// </summary>
    public string TimeZone { get; set; }
    /// <summary>
    /// Gets or sets the currency of the country.
    /// </summary>
    public Currency Currency { get; set; }
}
