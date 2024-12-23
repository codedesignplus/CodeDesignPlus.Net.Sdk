using System;
using System.Net;
using CodeDesignPlus.Net.Observability.Extensions;
using CodeDesignPlus.Net.xUnit.Containers.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace CodeDesignPlus.Net.Observability.Test.Extensions;

public class CheckServerTest
{

    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task AddObservability_InvokeDelegates_ExecuteServerSuccess(string environment)
    {
        // Arrange
        var testServer = CreateServer(environment);
        var client = testServer.CreateClient();

        // Act
        var response = await client.GetAsync("/api/test");
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello World!", message);

        while (InMemoryLogExporter.Exporters.IsEmpty || InMemoryMetricsExporter.Exporters.IsEmpty || InMemoryTraceExporter.Exporters.IsEmpty)
        {
            await Task.Delay(100);
        }

        Assert.NotEmpty(InMemoryLogExporter.Exporters);
        Assert.NotEmpty(InMemoryMetricsExporter.Exporters);
        Assert.NotEmpty(InMemoryTraceExporter.Exporters);

        InMemoryLogExporter.Exporters.Clear();
        InMemoryMetricsExporter.Exporters.Clear();
        InMemoryTraceExporter.Exporters.Clear();
    }

    private static TestServer CreateServer(string environment = "Development")
    {
        var configuration = xUnit.Extensions.ConfigurationUtil.GetConfiguration(new
        {
            Core = Helpers.ConfigurationUtil.CoreOptions,
            Observability = Helpers.ConfigurationUtil.ObservabilityOptions
        });

        return new TestServer(new WebHostBuilder()
            .UseEnvironment(environment)
            .UseConfiguration(configuration)
            .ConfigureLogging(logger =>
            {
                logger.AddConsole();
                logger.AddOpenTelemetry(logging =>
                {
                    logging.AddProcessor(new SimpleLogRecordExportProcessor(new InMemoryLogExporter()));
                    logging.AddConsoleExporter(x => x.Targets = ConsoleExporterOutputTargets.Console);
                });
            })
            .ConfigureServices((context, services) =>
            {
                services.AddObservability(
                    configuration,
                    context.HostingEnvironment,
                    metrics =>
                    {
                        metrics.AddReader(new PeriodicExportingMetricReader(new InMemoryMetricsExporter(), 1000));
                    },
                    tracing =>
                    {
                        tracing.AddProcessor(new BatchActivityExportProcessor(new InMemoryTraceExporter()));
                    }
                );
            })
            .Configure(x =>
            {
                x.Map("/api/test", app =>
                {
                    app.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<ServiceCollectionExtensionsTest>>();

                        logger.LogWarning("This is a warning log message");

                        await context.Response.WriteAsync("Hello World!");
                    });
                });
            })
        );

    }
}


