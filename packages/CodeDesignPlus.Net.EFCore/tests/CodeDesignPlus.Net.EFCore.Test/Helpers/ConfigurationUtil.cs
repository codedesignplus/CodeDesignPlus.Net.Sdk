namespace CodeDesignPlus.Net.EFCore.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly EFCoreOptions EFCoreOptions = new()
    {
        Enable = true,
        RegisterRepositories = true
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            EFCore = EFCoreOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
