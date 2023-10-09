namespace CodeDesignPlus.Net.Security.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly SecurityOptions SecurityOptions = new()
    {
        Enable = true,
        Name = nameof(Security.Options.SecurityOptions.Name),
        Email = $"{nameof(Security.Options.SecurityOptions.Name)}@codedesignplus.com"
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
