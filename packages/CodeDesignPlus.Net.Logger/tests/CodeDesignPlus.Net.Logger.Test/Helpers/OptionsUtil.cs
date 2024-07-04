using System.Text;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Logger.Options;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Logger.Test.Helpers;

public class OptionsUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        Business = nameof(Core.Abstractions.Options.CoreOptions.Business),
        AppName = nameof(Core.Abstractions.Options.CoreOptions.AppName),
        Version = "v1",
        Description = nameof(Core.Abstractions.Options.CoreOptions.Description),
        Contact = new Contact()
        {
            Name = nameof(Contact.Name),
            Email = "codedesignplus@outlook.com"
        },
    };

    public static readonly LoggerOptions loggerOptions = new()
    {
        Enable = true,
        OTelEndpoint = "http://localhost:4317",
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new 
        {
            Core = CoreOptions,
            Logger = loggerOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonConvert.SerializeObject(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }
}
