using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.EventStore.PubSub.Extensions;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Memory;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.xUnit.Output.Loggers;
using Microsoft.AspNetCore.Builder;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers;


public class Startup(IConfiguration configuration)
{
    public readonly static MemoryService MemoryService = new();
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder =>
            {
                builder.AddConsole()
                    .SetMinimumLevel(LogLevel.Trace)
                    .UsesScopes();
            })
            .AddCore(this.Configuration)
            .AddSingleton<IMemoryService>(x => MemoryService)
            .AddEventStore(this.Configuration)
            .AddEventStorePubSub(this.Configuration);
    }

    public static void Configure(IApplicationBuilder app)
    {

    }
}