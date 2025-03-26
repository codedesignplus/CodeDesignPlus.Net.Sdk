namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents the location information.
/// </summary>
public class Location
{
    /// <summary>
    /// Gets or sets the country information.
    /// </summary>
    public Country Country { get; set; }
    /// <summary>
    /// Gets or sets the state information.
    /// </summary>
    public State State { get; set; }
    /// <summary>
    /// Gets or sets the city information.
    /// </summary>
    public City City { get; set; }
    /// <summary>
    /// Gets or sets the locality information.
    /// </summary>
    public Locality Locality { get; set; }
    /// <summary>
    /// Gets or sets the neighborhood information.
    /// </summary>
    public Neighborhood Neighborhood { get; set; }
}
