namespace CodeDesignPlus.Net.RabitMQ.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly RabitMQOptions RabitMQOptions = new()
    {
        Enable = true,
        Host = nameof(RabitMQ.Abstractions.Options.RabitMQOptions.Host),
        UserName = $"{nameof(RabitMQ.Abstractions.Options.RabitMQOptions.Host)}@codedesignplus.com"
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            RabitMQ = RabitMQOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
