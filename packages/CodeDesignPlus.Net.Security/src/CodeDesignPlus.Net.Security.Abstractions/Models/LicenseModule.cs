namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents a module included in a tenant's license snapshot.
/// </summary>
public class LicenseModule
{
    /// <summary>
    /// Gets or sets the unique identifier for the module.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the module.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the description of the module.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
