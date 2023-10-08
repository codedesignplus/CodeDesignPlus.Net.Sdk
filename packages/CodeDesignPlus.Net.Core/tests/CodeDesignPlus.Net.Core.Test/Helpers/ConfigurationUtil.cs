namespace CodeDesignPlus.Net.Core.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        Enable = true,
        Name = nameof(Core.Options.CoreOptions.Name),
        Email = $"{nameof(Core.Options.CoreOptions.Name)}@codedesignplus.com"
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
