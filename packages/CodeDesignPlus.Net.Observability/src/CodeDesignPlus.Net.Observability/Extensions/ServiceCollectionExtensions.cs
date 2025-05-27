using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Observability.Abstractions.Options;
using CodeDesignPlus.Net.Observability.Exceptions;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CodeDesignPlus.Net.Observability.Extensions;

/// <summary>
/// Provides extension methods for adding observability services to the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds observability services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The host environment.</param>
    /// <param name="metricsBuilder">Optional metrics builder action.</param>
    /// <param name="traceBuilder">Optional trace builder action.</param>
    /// <returns>The OpenTelemetry builder.</returns>
    /// <exception cref="ObservabilityException">Thrown when the observability section is missing in the configuration.</exception>
    public static IOpenTelemetryBuilder AddObservability(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment, Action<MeterProviderBuilder> metricsBuilder = null, Action<TracerProviderBuilder> traceBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        var observabilitySection = configuration.GetSection(ObservabilityOptions.Section);

        if (!observabilitySection.Exists())
            throw new ObservabilityException($"The section {ObservabilityOptions.Section} is required.");

        var observabilityOptions = observabilitySection.Get<ObservabilityOptions>();
        var coreOptions = configuration.GetSection(CoreOptions.Section).Get<CoreOptions>();

        services
            .AddOptions<ObservabilityOptions>()
            .Bind(observabilitySection)
            .ValidateDataAnnotations();

        services.AddCore(configuration);

        var otel = services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(serviceName: coreOptions.AppName, serviceVersion: coreOptions.Version);
                resource.AddTelemetrySdk();
                resource.AddEnvironmentVariableDetector();
            });

        otel.ConfigureMetrics(environment, observabilityOptions, metricsBuilder);
        otel.ConfigureTracing(observabilityOptions, environment, traceBuilder);

        return otel;
    }

    /// <summary>
    /// Configures metrics for OpenTelemetry.
    /// </summary>
    /// <param name="otel">The OpenTelemetry builder.</param>
    /// <param name="environment">The host environment.</param>
    /// <param name="observabilityOptions">The observability options.</param>
    /// <param name="builder">Optional metrics builder action.</param>
    private static void ConfigureMetrics(this IOpenTelemetryBuilder otel, IHostEnvironment environment, ObservabilityOptions observabilityOptions, Action<MeterProviderBuilder> builder)
    {
        if (!observabilityOptions.Metrics.Enable)
            return;

        otel.WithMetrics(metricsBuilder =>
        {
            metricsBuilder.AddMetricsAspNetCoreInstrumentation(observabilityOptions.Metrics.AspNetCore);

            metricsBuilder.AddOtlpExporter(x =>
            {
                x.Endpoint = observabilityOptions.ServerOtel;
                x.Protocol = OtlpExportProtocol.Grpc;
            });

            if (environment.IsDevelopment())
                metricsBuilder.AddConsoleExporter();

            builder?.Invoke(metricsBuilder);
        });
    }

    /// <summary>
    /// Adds ASP.NET Core instrumentation for metrics.
    /// </summary>
    /// <param name="metricsBuilder">The metrics builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddMetricsAspNetCoreInstrumentation(this MeterProviderBuilder metricsBuilder, bool enable)
    {
        if (enable)
            metricsBuilder.AddAspNetCoreInstrumentation();
    }

    /// <summary>
    /// Configures tracing for OpenTelemetry.
    /// </summary>
    /// <param name="otel">The OpenTelemetry builder.</param>
    /// <param name="observabilityOptions">The observability options.</param>
    /// <param name="environment">The host environment.</param>
    /// <param name="builder">Optional trace builder action.</param>
    private static void ConfigureTracing(this IOpenTelemetryBuilder otel, ObservabilityOptions observabilityOptions, IHostEnvironment environment, Action<TracerProviderBuilder> builder)
    {
        if (!observabilityOptions.Trace.Enable)
            return;

        otel.WithTracing(tracing =>
        {
            tracing.AddTraceAspNetCoreInstrumentation(observabilityOptions.Trace.AspNetCore);
            tracing.AddTraceGrpcClientInstrumentation(observabilityOptions.Trace.GrpcClient);
            tracing.AddTraceSqlClientInstrumentation(observabilityOptions.Trace.SqlClient);
            tracing.AddTraceCodeDesignPlusSdkInstrumentation(observabilityOptions.Trace.CodeDesignPlusSdk);
            tracing.AddTraceRedisInstrumentation(observabilityOptions.Trace.Redis);
            tracing.AddTraceKafkaInstrumentation(observabilityOptions.Trace.Kafka);
            tracing.AddTraceRabbitMQInstrumentation(observabilityOptions.Trace.RabbitMQ);

            tracing.AddOtlpExporter(x =>
            {
                x.Endpoint = observabilityOptions.ServerOtel;
                x.Protocol = OtlpExportProtocol.Grpc;
            });

            if (environment.IsDevelopment())
                tracing.AddConsoleExporter();

            builder?.Invoke(tracing);
        });
    }

    /// <summary>
    /// Adds ASP.NET Core instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceAspNetCoreInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
        {
            tracing.AddAspNetCoreInstrumentation(options =>
            {
                options.Filter = httpContext => httpContext.Request.Path.StartsWithSegments("/api");
            });

            tracing.AddHttpClientInstrumentation();
        }
    }

    /// <summary>
    /// Adds gRPC client instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceGrpcClientInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
        {
            tracing.AddGrpcClientInstrumentation();
            tracing.AddHttpClientInstrumentation();
        }
    }

    /// <summary>
    /// Adds SQL client instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceSqlClientInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddSqlClientInstrumentation();
    }

    /// <summary>
    /// Adds CodeDesignPlus SDK instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceCodeDesignPlusSdkInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
        {
            tracing.AddSource("CodeDesignPlus.Net.Mongo.Diagnostics");
            tracing.AddSource("CodeDesignPlus.Net.PubSub");
        }
    }

    /// <summary>
    /// Adds Redis instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceRedisInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddRedisInstrumentation();
    }

    /// <summary>
    /// Adds Kafka instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceKafkaInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddConfluentKafkaInstrumentation();
    }
    
    /// <summary>
    /// Adds RabbitMQ instrumentation for tracing.
    /// </summary>
    /// <param name="tracing">The tracing builder.</param>
    /// <param name="enable">Indicates whether to enable the instrumentation.</param>
    private static void AddTraceRabbitMQInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddRabbitMQInstrumentation();
    }
}