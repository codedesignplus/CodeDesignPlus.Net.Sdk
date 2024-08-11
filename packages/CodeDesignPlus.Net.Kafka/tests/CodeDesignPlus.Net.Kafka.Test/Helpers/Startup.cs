using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Kafka.Extensions;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Microsoft.AspNetCore.Builder;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers;


public class Startup(IConfiguration configuration)
{
    private static MemoryService memoryService = new();

    public static MemoryService MemoryService { get => memoryService; set => memoryService = value; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder =>
            {
                builder.AddConsole()
                    .SetMinimumLevel(LogLevel.Trace)
                    .UsesScopes();
            })
            .AddCore(configuration)
            .AddPubSub(configuration)
            .AddSingleton<IMemoryService>(x => MemoryService)
            .AddKafka(configuration);
    }

    public static void Configure(IApplicationBuilder app)
    {

    }
}