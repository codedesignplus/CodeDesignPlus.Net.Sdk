namespace CodeDesignPlus.Net.Mongo.Diagnostics.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly MongoDiagnosticsOptions MongoDiagnosticsOptions = new()
    {
        Enable = true,
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            MongoDiagnostics = MongoDiagnosticsOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
