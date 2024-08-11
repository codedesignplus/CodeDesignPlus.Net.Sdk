using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Options;

/// <summary>
/// Options to setting of the PubSub
/// </summary>
public class PubSubOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "PubSub";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool UseQueue { get; set; } = true;
    /// <summary>
    /// Number of seconds to wait for the queue
    /// </summary>
    [Range(1, 10)]
    public uint SecondsWaitQueue { get; set; } = 2;
    /// <summary>
    /// Gets or sets the enable diagnostic
    /// </summary>
    public bool EnableDiagnostic { get; set; } = false;
    /// <summary>
    /// Gets or sets the register automatic handlers
    /// </summary>
    public bool RegisterAutomaticHandlers { get; set; } = true;
}
