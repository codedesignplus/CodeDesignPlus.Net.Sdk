using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.EventStore.PubSub.Extensions;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Memory;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Microsoft.AspNetCore.Builder;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers;


public class Startup
{
    public readonly static MemoryService MemoryService = new();
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(builder =>
            {
                builder.AddConsole()
                    .SetMinimumLevel(LogLevel.Trace)
                    .UsesScopes();
            })
            .AddPubSub(this.Configuration)
            .AddSingleton<IMemoryService>(x => MemoryService)
            .AddEventSourcing(this.Configuration)
            .AddEventStore(this.Configuration)
            .AddEventStorePubSub(this.Configuration);
    }

    public void Configure(IApplicationBuilder app)
    {

    }
}