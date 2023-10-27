namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly EventSourcingOptions EventSourcingOptions = new()
    {
        Enable = true,
        Name = nameof(Sourcing.Options.EventSourcingOptions.Name),
        Email = $"{nameof(Sourcing.Options.EventSourcingOptions.Name)}@codedesignplus.com"
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            EventSourcing = EventSourcingOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
