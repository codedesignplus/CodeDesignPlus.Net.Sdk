namespace CodeDesignPlus.Net.EFCore.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly EFCoreOptions EFCoreOptions = new()
    {
        ClaimsIdentity = new ClaimsOption()
        {
            Email = nameof(ClaimsOption.Email),
            IdUser = nameof(ClaimsOption.IdUser),
            Role = nameof(ClaimsOption.Role),
            User = nameof(ClaimsOption.User),
        }
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
