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
}
