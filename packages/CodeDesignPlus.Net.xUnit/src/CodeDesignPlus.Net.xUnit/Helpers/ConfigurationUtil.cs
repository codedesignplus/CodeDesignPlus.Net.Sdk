﻿using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace CodeDesignPlus.Net.xUnit.Helpers;

public static class ConfigurationUtil
{
    public static IConfiguration GetConfiguration(object appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
