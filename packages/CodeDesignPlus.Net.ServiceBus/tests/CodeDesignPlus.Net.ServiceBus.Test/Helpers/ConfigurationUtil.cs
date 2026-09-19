using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.ServiceBus.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "test-servicebus",
        TypeEntryPoint = "rest",
        Business = "CodeDesignPlus",
        Description = "Test Azure Service Bus",
        Version = "v1",
        Contact = new Contact()
        {
            Name = "CodeDesignPlus",
            Email = "wliscano@codedesignplus.com"
        }
    };

    public static readonly ServiceBusOptions ServiceBusOptions = new()
    {
        Enable = true,
        FullyQualifiedNamespace = "test.servicebus.windows.net",
        RegisterHealthCheck = false
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new
        {
            Core = CoreOptions,
            ServiceBus = ServiceBusOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }
}
