namespace CodeDesignPlus.Net.Core.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        Enable = true,
        AppName = nameof(Core.Options.CoreOptions.AppName),
        Email = $"{nameof(Core.Options.CoreOptions.AppName)}@codedesignplus.com"
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            Core = CoreOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
