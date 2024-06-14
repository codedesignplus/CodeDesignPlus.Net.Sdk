using System.Text;
using System.Text.Json;
using CodeDesignPlus.PubSub.Without.PubSubService.Test.Helpers.Models;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using Microsoft.Extensions.Configuration;

namespace CodeDesignPlus.PubSub.Without.PubSubService.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly PubSubOptions PubSubOptions = new()
    {
        UseQueue = true,
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
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
