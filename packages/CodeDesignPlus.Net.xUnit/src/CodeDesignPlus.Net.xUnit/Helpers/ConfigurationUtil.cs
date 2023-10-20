using System.Text;
using System.Text.Json;
using CodeDesignPlus.Net.xUnit.Helpers.Models;
using Microsoft.Extensions.Configuration;

namespace CodeDesignPlus.Net.xUnit.Helpers;

public static class ConfigurationUtil<TOptions>
{
    public static IConfiguration GetConfiguration(TOptions options)
    {
        return GetConfiguration(new AppSettings<TOptions>()
        {
            Section = options
        });
    }

    public static IConfiguration GetConfiguration(AppSettings<TOptions> appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
