namespace CodeDesignPlus.Net.Redis.PubSub.Abstractions.Options;

/// <summary>
/// Represents the configuration options for Redis Pub/Sub.
/// </summary>
public class RedisPubSubOptions : PubSubOptions
{
    /// <summary>
    /// The name of the configuration section used in the appsettings.
    /// </summary>
    public static new readonly string Section = "RedisPubSub";

    /// <summary>
    /// Gets or sets a value indicating whether Redis Pub/Sub is enabled.
    /// </summary>
    public bool Enable { get; set; }
}