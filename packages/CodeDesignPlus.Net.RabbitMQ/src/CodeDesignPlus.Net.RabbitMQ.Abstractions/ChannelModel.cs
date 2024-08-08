namespace CodeDesignPlus.Net.RabbitMQ.Abstractions;

public class ChannelModel
{
    public string Key { get; private set; }
    public IModel Channel { get; private set; }
    public string ConsumerTag { get; set; }

    private ChannelModel(string key, IModel channel)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(channel);

        this.Key = key;
        this.Channel = channel;
    }

    public static ChannelModel Create(string key, IModel channel)
    {
        return new ChannelModel(key, channel);
    }
}
