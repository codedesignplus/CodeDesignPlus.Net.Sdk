namespace CodeDesignPlus.Net.PubSub.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly PubSubOptions PubSubOptions = new()
    {
        UseQueue = true,
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            PubSub = PubSubOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
