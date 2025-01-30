namespace CodeDesignPlus.Net.RabbitMQ.Abstractions;

/// <summary>
/// Represents a model for a RabbitMQ channel.
/// </summary>
public class ChannelModel
{
    /// <summary>
    /// Gets the key associated with the channel.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// Gets the RabbitMQ channel.
    /// </summary>
    public IChannel Channel { get; private set; }

    /// <summary>
    /// Gets or sets the consumer tag for the channel.
    /// </summary>
    public string ConsumerTag { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelModel"/> class.
    /// </summary>
    /// <param name="key">The key associated with the channel.</param>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> or <paramref name="channel"/> is null.</exception>
    private ChannelModel(string key, IChannel channel)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(channel);

        this.Key = key;
        this.Channel = channel;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ChannelModel"/> class.
    /// </summary>
    /// <param name="key">The key associated with the channel.</param>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <returns>A new instance of the <see cref="ChannelModel"/> class.</returns>
    public static ChannelModel Create(string key, IChannel channel)
    {
        return new ChannelModel(key, channel);
    }
}