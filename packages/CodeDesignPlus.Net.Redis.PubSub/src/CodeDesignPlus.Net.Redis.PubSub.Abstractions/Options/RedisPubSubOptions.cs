namespace CodeDesignPlus.Net.Redis.PubSub.Abstractions.Options;

/// <summary>
/// Options to setting of the Redis.PubSub
/// </summary>
public class RedisPubSubOptions: PubSubOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static new readonly string Section = "RedisPubSub";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
}
