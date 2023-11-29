namespace CodeDesignPlus.Net.Security.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly SecurityOptions SecurityOptions = new()
    {
        Authority = new Uri("https://localhost:5001")
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            Security = SecurityOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
