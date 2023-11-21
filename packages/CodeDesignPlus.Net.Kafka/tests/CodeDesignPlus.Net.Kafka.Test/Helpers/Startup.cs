using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using Microsoft.AspNetCore.Builder;
using CodeDesignPlus.Net.Kafka.Extensions;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Microsoft.AspNetCore.Http;
using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers;


public class Startup
{
    public static MemoryService MemoryService = new();
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
            .AddKafka(this.Configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
        // app.UseRouting();

        // app.UseEndpoints(endpoints =>
        // {
        //     endpoints.MapPost("/publish", async context =>
        //     {
        //         var pubSub = context.RequestServices.GetRequiredService<IEventBus>();
        //         await pubSub.PublishAsync(new StartupLogic.StartupMessage()); // Ejemplo de publicación

                


        //         await context.Response.WriteAsync("Mensaje publicado!"); // Ejemplo de respuesta
        //     });
        // });
    }
}