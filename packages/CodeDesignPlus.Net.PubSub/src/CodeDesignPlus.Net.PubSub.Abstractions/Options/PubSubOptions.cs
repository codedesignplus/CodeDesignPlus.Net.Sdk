using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Options;

/// <summary>
/// Options for configuring the PubSub service.
/// </summary>
public class PubSubOptions
{
    /// <summary>
    /// The name of the section used in the appsettings.
    /// </summary>
    public static readonly string Section = "PubSub";

    /// <summary>
    /// Gets or sets a value indicating whether to use a queue for event handling.
    /// </summary>
    public bool UseQueue { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of seconds to wait for the queue.
    /// </summary>
    [Range(1, 10)]
    public uint SecondsWaitQueue { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether diagnostics are enabled.
    /// </summary>
    public bool EnableDiagnostic { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to register automatic handlers.
    /// </summary>
    public bool RegisterAutomaticHandlers { get; set; } = true;
}