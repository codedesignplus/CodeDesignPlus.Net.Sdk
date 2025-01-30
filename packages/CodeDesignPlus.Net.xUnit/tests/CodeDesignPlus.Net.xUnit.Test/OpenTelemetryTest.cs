using CodeDesignPlus.Net.xUnit.Containers.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.xUnit.Test;

public class OpenTelemetryTest : IClassFixture<OpenTelemetryContainer>
{
    private readonly TestServer server;
    private readonly HttpClient client;

    public OpenTelemetryTest(OpenTelemetryContainer container)
    {
        var configuration = ConfigurationUtil.GetConfiguration(new { });

        this.server = new TestServer(new WebHostBuilder()
            .UseConfiguration(configuration)
            .ConfigureLogging(logger =>
            {
                logger.AddOpenTelemetry(logging =>
                {
                    logging.AddOtlpExporter();
                    logging.AddProcessor(new SimpleLogRecordExportProcessor(new InMemoryLogExporter()));
                    logging.AddConsoleExporter(x => x.Targets = ConsoleExporterOutputTargets.Console);
                });
            })
            .ConfigureServices(x =>
            {
                x.AddOpenTelemetry()
                    .ConfigureResource(resource =>
                    {
                        resource.AddService("MyServiceName", "1.0.0");
                    })

                    .WithTracing(tracing => tracing
                        .AddProcessor(new BatchActivityExportProcessor(new InMemoryTraceExporter()))
                        .AddOtlpExporter())

                    .WithMetrics(metrics => metrics
                        .AddOtlpExporter()
                        .AddConsoleExporter()
                        .AddReader(new PeriodicExportingMetricReader(new InMemoryMetricsExporter(), 1000)));
            })
            .Configure(x =>
            {
                x.Map("/api/test", app =>
                {
                    app.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<OpenTelemetryTest>>();

                        logger.LogWarning("This is a warning log message");

                        await context.Response.WriteAsync("Hello World!");
                    });
                });
            })
        );

        this.client = server.CreateClient();
    }

    [Fact]
    public async Task CheckService()
    {
        var response = await client.GetAsync("/api/test");

        response.EnsureSuccessStatusCode();


        Assert.False(InMemoryLogExporter.Exporters.IsEmpty);
    }

}
