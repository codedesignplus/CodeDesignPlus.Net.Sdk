using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "test-rabbitmq",
        Business = "CodeDesignPlus",
        Description = "Test RabbitMQ",
        Version = "v1",
        Contact = new Contact()
        {
            Name = "CodeDesignPlus",
            Email = "codedesignplus@outlook.com"
        }
    };

    public static readonly RabbitMQOptions RabbitMQOptions = new()
    {
        Enable = true,
        Host = nameof(RabbitMQ.Abstractions.Options.RabbitMQOptions.Host),
        UserName = $"{nameof(RabbitMQ.Abstractions.Options.RabbitMQOptions.Host)}@codedesignplus.com",
        Password = nameof(RabbitMQ.Abstractions.Options.RabbitMQOptions.Password),
        Port = 5672,
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            RabbitMQ = RabbitMQOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
