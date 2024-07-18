using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Observability.Abstractions.Options;
using CodeDesignPlus.Net.Observability.Exceptions;
using CodeDesignPlus.Net.Observability.Services;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CodeDesignPlus.Net.Observability.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.EFCore
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add CodeDesignPlus.EFCore configuration options
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var observabilitySection = configuration.GetSection(ObservabilityOptions.Section);
        var coreSection = configuration.GetSection(CoreOptions.Section);

        if (!observabilitySection.Exists())
            throw new ObservabilityException($"The section {ObservabilityOptions.Section} is required.");

        if (!coreSection.Exists())
            throw new ObservabilityException($"The section {CoreOptions.Section} is required.");

        var observabilityOptions = observabilitySection.Get<ObservabilityOptions>();
        var coreOptions = coreSection.Get<CoreOptions>();

        services
            .AddOptions<ObservabilityOptions>()
            .Bind(observabilitySection)
            .ValidateDataAnnotations();

        services.AddSingleton<IObservabilityService, ObservabilityService>();

        var otel = services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource =>
        {
            resource.AddService(serviceName: coreOptions.AppName);
        });

        // Custom metrics for the application
        var greeterMeter = new Meter("OtPrGrYa.Example", "1.0.0");
        var countGreetings = greeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");

        // Custom ActivitySource for the application
        var greeterActivitySource = new ActivitySource("OtPrGrJa.Example");

        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            .AddMeter(greeterMeter.Name)
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddPrometheusExporter());

        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation(options =>
            {
                options.Filter = (httpContext) => httpContext.Request.Path.StartsWithSegments("/api");
            });
            tracing.AddHttpClientInstrumentation();

            //tracing.AddGrpcClientInstrumentation();

            tracing.AddSource(greeterActivitySource.Name);

            tracing.AddSource("CodeDesignPlus.Net.Mongo.Diagnostics");

            tracing.AddSource("CodeDesignPlus.Net.PubSub");

            tracing.AddRedisInstrumentation();

            tracing.AddConfluentKafkaInstrumentation();

            tracing.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = observabilityOptions.Endpoint;
            });

            // TODO: local development only
            tracing.AddConsoleExporter();
        });




        return services;
    }


    public static WebApplication UseObservability(this WebApplication app)
    {
        // Configure the Prometheus scraping endpoint
        app.MapPrometheusScrapingEndpoint();

        return app;
    }
}
