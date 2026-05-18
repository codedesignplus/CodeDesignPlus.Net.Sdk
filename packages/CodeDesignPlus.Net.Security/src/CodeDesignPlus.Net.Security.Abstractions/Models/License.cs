using NodaTime;

namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents the license information.
/// </summary>
public class License
{
    /// <summary>
    /// Gets or sets the identifier of the license.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the license.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the start date of the license.
    /// </summary>
    public Instant StartDate { get; set; }
    /// <summary>
    /// Gets or sets the expiration date of the license.
    /// </summary>
    public Instant ExpirationDate { get; set; }
    /// <summary>
    /// Gets or sets the metadata information of the license.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = [];
    /// <summary>
    /// Gets or sets the modules active in this license snapshot.
    /// This is an immutable capture of the modules purchased at order time.
    /// </summary>
    public List<LicenseModule> Modules { get; set; } = [];
}
