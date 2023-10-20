namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly RedisEventBusOptions RedisEventBusOptions = new()
    {
        Enable = true,
        Name = nameof(Redis.Event.Bus.Options.RedisEventBusOptions.Name),
        Email = $"{nameof(Redis.Event.Bus.Options.RedisEventBusOptions.Name)}@codedesignplus.com"
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            RedisEventBus = RedisEventBusOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
