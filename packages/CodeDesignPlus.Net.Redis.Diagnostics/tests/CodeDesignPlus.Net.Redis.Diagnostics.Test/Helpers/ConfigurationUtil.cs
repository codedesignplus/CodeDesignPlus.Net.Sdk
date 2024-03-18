namespace CodeDesignPlus.Net.Redis.Diagnostics.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly RedisDiagnosticsOptions RedisDiagnosticsOptions = new()
    {
        Enable = true,
        Name = nameof(Diagnostics.Options.RedisDiagnosticsOptions.Name),
        Email = $"{nameof(Diagnostics.Options.RedisDiagnosticsOptions.Name)}@codedesignplus.com"
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            RedisDiagnostics = RedisDiagnosticsOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
