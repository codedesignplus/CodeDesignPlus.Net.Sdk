namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;

/// <summary>
/// Represents the configuration options for the resources.
/// </summary>
public class ResourcesOptions
{
    /// <summary>
    /// The section name in the configuration file.
    /// </summary>
    public static string Section => "Resources";
    /// <summary>
    /// Gets or sets a value indicating whether the resources are enabled.
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Gets or sets the server address where the resources are registered.
    /// </summary>
    public Uri Server { get; set; }
    /// <summary>
    /// Gets or sets the wait before the first retry when the registration fails. It doubles on each failed attempt up
    /// to <see cref="RetryMaxDelay"/>.
    /// </summary>
    public TimeSpan RetryInitialDelay { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// Gets or sets the longest wait between two registration attempts.
    /// </summary>
    public TimeSpan RetryMaxDelay { get; set; } = TimeSpan.FromMinutes(5);
}
