namespace CodeDesignPlus.Net.Mongo.Diagnostics.Abstractions.Options;

/// <summary>
/// Options to setting of the Mongo.Diagnostics
/// </summary>
public class MongoDiagnosticsOptions 
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "MongoDiagnostics";

    /// <summary>
    /// Get or set the enable of the diagnostics
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public bool EnableCommandText { get; set; }
}
