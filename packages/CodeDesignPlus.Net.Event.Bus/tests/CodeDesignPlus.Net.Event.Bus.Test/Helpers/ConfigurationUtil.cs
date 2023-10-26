namespace CodeDesignPlus.Net.Event.Bus.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly EventBusOptions EventBusOptions = new()
    {
        EnableQueue = true,
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            EventBus = EventBusOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
