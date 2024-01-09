
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;

namespace CodeDesignPlus.Net.Logger.Test;

public class HostBuilderExtensionTest
{
    [Fact]
    public void UseSiigoSerilog_RegisterLogger_Succecss()
    {
        // Arrange
        var hostBuilder = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Helpers"));
                config.AddEnvironmentVariables();

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {

            })
            .UseSerilog(configuration =>
            {
                configuration.WriteTo.InMemory(
               outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
               restrictedToMinimumLevel: LogEventLevel.Verbose);
            });

        // Act
        var host = hostBuilder.Build();

        var logger = host.Services.GetService<ILogger<HostBuilderExtensionTest>>();

        logger!.LogDebug("Hola");

        // Assert
        Assert.NotNull(logger);

        Assert.Contains(InMemorySink.Instance.LogEvents, e => e.RenderMessage() == "Hola");

        Assert.Contains(InMemorySink.Instance.LogEvents, e =>
            e.Properties.ContainsKey("SourceContext") &&
            e.Properties.ContainsKey("MachineName") &&
            e.Properties.ContainsKey("ThreadId") &&
            e.Properties.ContainsKey("ThreadName") &&
            e.Properties.ContainsKey("ProcessId") &&
            e.Properties.ContainsKey("ProcessName") &&
            e.Properties.ContainsKey("EnvironmentUserName") &&
            e.Properties.ContainsKey("AppName")
        );
    }

    
    [Fact]
    public void UseSiigoSerilog_ActionIsNull_Succecss()
    {
        // Arrange
        var hostBuilder = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Helpers"));
                config.AddEnvironmentVariables();

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {

            })
            .UseSerilog();

        // Act
        var host = hostBuilder.Build();

        var logger = host.Services.GetService<ILogger<HostBuilderExtensionTest>>();

        logger!.LogDebug("Hola");

        // Assert
        Assert.NotNull(logger);
    }
}
