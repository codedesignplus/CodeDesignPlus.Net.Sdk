namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents a city.
/// </summary>
public class City
{
    /// <summary>
    /// Gets or sets the identifier of the city.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the city.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the time zone of the city.
    /// </summary>
    public string TimeZone { get; set; }
}
