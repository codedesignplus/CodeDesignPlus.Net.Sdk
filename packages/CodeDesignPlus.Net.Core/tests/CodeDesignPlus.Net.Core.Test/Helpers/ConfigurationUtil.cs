﻿using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.Core.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly CoreOptions CoreOptions = new()
    {
        Business = nameof(Core.Abstractions.Options.CoreOptions.Business),
        AppName = "ms-test",
        Version = "v1",
        Description = nameof(Core.Abstractions.Options.CoreOptions.Description),
        Contact = new Contact()
        {
            Name = nameof(Contact.Name),
            Email = "codedesignplus@outlook.com"
        },
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            Core = CoreOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
