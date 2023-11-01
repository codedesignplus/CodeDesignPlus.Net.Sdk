using CodeDesignPlus.Net.Event.Bus.Extensions;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Memory;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Microsoft.AspNetCore.Builder;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers;


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
            .AddEventBus(this.Configuration)
            .AddSingleton<IMemoryService>(x => MemoryService)
            .AddEventSourcing(this.Configuration)
            .AddEventStore(this.Configuration);
    }

    public void Configure(IApplicationBuilder app)
    {

    }
}