
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Logger.Extensions;
using CodeDesignPlus.Net.Logger.Options;
using CodeDesignPlus.Net.Logger.Test.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CodeDesignPlus.Net.Serializers;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Xunit.Sdk;

namespace CodeDesignPlus.Net.Logger.Test.Extensions;

[Collection(ObservabilityCollectionFixture.Collection)]
public class ServiceCollectionExtensionTest 
{
    public ServiceCollectionExtensionTest(ObservabilityCollectionFixture fixture)
    {
        if(!fixture.Container.IsRunning)
            throw new TestClassException("The observability container is not running.");
    }

    [Fact]
    public void AddLogger_ThrowArgumentNulException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;
        IConfiguration configuration = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddLogger(configuration));
    }

    [Fact]
    public void AddLogger_ThrowArgumentNulException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddLogger(configuration));
    }

    [Fact]
    public void AddLogger_ThrowLoggerException_WhenSectionNotExists()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        var exception = Assert.Throws<Logger.Exceptions.LoggerException>(() => services.AddLogger(configuration));

        // Assert
        Assert.Equal("The section Logger is required.", exception.Message);
    }

    [Fact]
    public void AddLogger_CheckServices_ReturnServices()
    {
        // Arrange
        var configuration = OptionsUtil.GetConfiguration();

        var services = new ServiceCollection();
        services.AddTransient((x) => configuration);
        services.AddLogger(configuration);

        // Act
        var loggerOptions = services.BuildServiceProvider().GetService<IOptions<LoggerOptions>>();
        var coreOptions = services.BuildServiceProvider().GetService<IOptions<CoreOptions>>();

        // Assert
        Assert.NotNull(coreOptions);
        Assert.NotNull(loggerOptions);
        Assert.True(loggerOptions.Value.Enable);
        Assert.Equal("http://localhost:4317", loggerOptions.Value.OTelEndpoint);
    }

    [Fact]
    public void UseSerilog_RegisterLogger_Succecss()
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
                   restrictedToMinimumLevel: LogEventLevel.Verbose
               );
            });

        // Act
        var host = hostBuilder.Build();

        var logger = host.Services.GetService<ILogger<ServiceCollectionExtensionTest>>();

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
    public async Task UseSerilog_SendsLogsToOpenTelemetryAndLoki_Success()
    {
        // Arrange
        var hostBuilder = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Helpers"));
                config.AddEnvironmentVariables();

                config.AddJsonFile("appsettings-otel.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddCore(context.Configuration);
                services.AddLogger(context.Configuration);
            })
            .UseSerilog(configuration =>
            {
                configuration.WriteTo.InMemory(
                   outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                   restrictedToMinimumLevel: LogEventLevel.Verbose
               );
            });

        // Act
        var host = hostBuilder.Build();

        var logger = host.Services.GetRequiredService<ILogger<ServiceCollectionExtensionTest>>();

        logger.LogInformation("This is a test log for integration testing.");

        await Task.Delay(5000);

        var url = "http://localhost:3100/loki/api/v1/query?query={job=\"ms-test\"}";

        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonSerializer.Deserialize<RootObject>(content);

        Assert.NotNull(jsonResponse);
        Assert.Equal("success", jsonResponse.Status);

        var result = jsonResponse.Data.Result.FirstOrDefault();

        Assert.NotNull(result);
        Assert.NotEmpty(result.Values);

        var jsonValues = result.Values.FirstOrDefault();

        Assert.NotNull(jsonValues);
        var responseLog = JsonSerializer.Deserialize<LogEntry>(jsonValues.LastOrDefault()!);


        Assert.NotNull(responseLog);
        Assert.Equal("Information", responseLog.Severity);
        Assert.Equal("This is a test log for integration testing.", responseLog.Body);
        Assert.Equal("ms-test", responseLog.Attributes!.AppName);
        Assert.NotNull(responseLog.Attributes!.EnvironmentUserName);
        Assert.NotNull(responseLog.Attributes!.MachineName);
        Assert.NotEqual(0, responseLog.Attributes!.ProcessId);
        Assert.NotNull(responseLog.Attributes!.ProcessName);
        Assert.NotEqual(0, responseLog.Attributes!.ThreadId);
        Assert.NotNull(responseLog.Attributes!.ThreadName);
        Assert.Equal("CodeDesignPlus", responseLog.Resources!.ServiceBusiness);
        Assert.Equal("codedesignplus@outlook.com", responseLog.Resources!.ServiceContactEmail);
        Assert.Equal("CodeDesignPlus", responseLog.Resources!.ServiceContactName);
        Assert.Equal("unit test for CodeDesignPlus.Net.Logger", responseLog.Resources!.ServiceDescription);
        Assert.Equal("ms-test", responseLog.Resources!.ServiceName);
        Assert.Equal("v1", responseLog.Resources!.ServiceVersion);
    }

    [Fact]
    public void UseSerilog_ActionIsNull_Succecss()
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

        var logger = host.Services.GetService<ILogger<ServiceCollectionExtensionTest>>();

        logger!.LogDebug("Hola");

        // Assert
        Assert.NotNull(logger);
    }
}
