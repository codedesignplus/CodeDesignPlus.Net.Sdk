using CodeDesignPlus.Net.xUnit.Microservice.Server.Logger;
using CodeDesignPlus.Net.xUnit.Microservice.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.xUnit.Microservice.Server.Authentication;

namespace CodeDesignPlus.Net.xUnit.Microservice.Server;

/// <summary>
/// A server class for configuring and managing a web application for testing purposes.
/// </summary>
/// <typeparam name="TProgram">The program class of the web application.</typeparam>
public class Server<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    /// <summary>
    /// Gets the server compose instance for managing external dependencies.
    /// </summary>
    public ServerCompose Compose { get; } = new();

    /// <summary>
    /// Gets the in-memory logger provider for capturing log messages.
    /// </summary>
    public InMemoryLoggerProvider LoggerProvider { get; } = new();

    /// <summary>
    /// Gets or sets the action to configure the in-memory collection.
    /// </summary>
    public Action<Dictionary<string, string>> InMemoryCollection { get; set; }

    /// <summary>
    /// Configures the web host for the application.
    /// </summary>
    /// <param name="builder">The web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(x =>
        {
            var configuration = new Dictionary<string, string>()
            {
                {"Redis:Instances:Core:ConnectionString", $"{Compose.Redis.Item1}:{Compose.Redis.Item2}"},
                {"RabbitMQ:Host", Compose.RabbitMQ.Item1},
                {"RabbitMQ:Port", Compose.RabbitMQ.Item2.ToString()},
                {"MongoDB:ConnectionString", $"mongodb://{Compose.Mongo.Item1}:{Compose.Mongo.Item2}"},
                {"Observability:ServerOtel", $"http://{Compose.Otel.Item1}:{Compose.Otel.Item2}"},
                {"Logger:OTelEndpoint", $"http://{Compose.Otel.Item1}:{Compose.Otel.Item2}" },
            };

            InMemoryCollection?.Invoke(configuration);

            x.AddInMemoryCollection(configuration);
        });

        builder.ConfigureServices(ConfigureServices);

        builder.UseEnvironment("Development");
    }

    /// <summary>
    /// Disposes the server and its resources.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the object is being disposed.</param>
    protected override void Dispose(bool disposing)
    {
        Compose.StopInstance();

        LoggerProvider.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication("TestAuth")
                .AddScheme<AuthenticationSchemeOptions, AuthHandler>("TestAuth", options => { });

        services.AddSingleton(this.LoggerProvider);
        services.AddSingleton<ILoggerFactory, InMemoryLoggerFactory>();
    }



    
}