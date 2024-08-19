namespace CodeDesignPlus.Net.Observability.Abstractions.Options;

/// <summary>
/// Represents the metrics options for configuring observability.
/// </summary>
public class Metrics
{
    /// <summary>
    /// Gets or sets a value indicating whether metrics are enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ASP.NET Core instrumentation is enabled for metrics.
    /// </summary>
    public bool AspNetCore { get; set; }
}