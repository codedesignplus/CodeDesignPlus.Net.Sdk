using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers;

public static class ConfigurationUtil
{
    
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "xunit-pub-sub",
        Description = "The xunit test for the pub sub library",
        Version = "v1",
        Business = "CodeDesignPlus",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "CodeDesignPlus@outlook.com"
        }
    };

    public static readonly PubSubOptions PubSubOptions = new()
    {
        UseQueue = true,
        EnableDiagnostic = true,
        SecondsWaitQueue = 2,
        RegisterAutomaticHandlers = true
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new
        {
            Core = CoreOptions,
            PubSub = PubSubOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
