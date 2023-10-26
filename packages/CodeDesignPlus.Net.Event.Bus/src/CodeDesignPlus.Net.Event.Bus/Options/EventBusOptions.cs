using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Event.Bus.Options;

/// <summary>
/// Options to setting of the Event.Bus
/// </summary>
public class EventBusOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "EventBus";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool EnableQueue { get; set; }
    /// <summary>
    /// Number of seconds to wait for the queue
    /// </summary>
    [Range(1, 10)]
    public uint SecondsWaitQueue { get; set; } = 2;
}
