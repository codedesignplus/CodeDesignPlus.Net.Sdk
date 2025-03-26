namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents the tenant information.
/// </summary>
public class Tenant
{
    /// <summary>
    /// Gets or sets the identifier of the tenant.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the tenant.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the domain of the tenant.
    /// </summary>
    public Uri Domain { get; set; }
    /// <summary>
    /// Gets or sets the license information of the tenant.
    /// </summary>
    public License License { get; set; }
    /// <summary>
    /// Gets or sets the location information of the tenant.
    /// </summary>
    public Location Location { get; set; }
    /// <summary>
    /// Gets or sets the metadata information of the tenant.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; }
}
