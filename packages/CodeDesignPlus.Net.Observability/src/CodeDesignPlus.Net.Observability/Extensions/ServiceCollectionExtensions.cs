using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Observability.Abstractions.Options;
using CodeDesignPlus.Net.Observability.Exceptions;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CodeDesignPlus.Net.Observability.Extensions;

public static class ServiceCollectionExtensions
{
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
            });

        otel.ConfigureMetrics(environment, observabilityOptions, metricsBuilder);
        otel.ConfigureTracing(observabilityOptions, environment, traceBuilder);

        return otel;
    }

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

    private static void AddMetricsAspNetCoreInstrumentation(this MeterProviderBuilder metricsBuilder, bool enable)
    {
        if (enable)
            metricsBuilder.AddAspNetCoreInstrumentation();
    }

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

    private static void AddTraceGrpcClientInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
        {
            tracing.AddGrpcClientInstrumentation();
            tracing.AddHttpClientInstrumentation();
        }
    }

    private static void AddTraceSqlClientInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddSqlClientInstrumentation();
    }

    private static void AddTraceCodeDesignPlusSdkInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
        {
            tracing.AddSource("CodeDesignPlus.Net.Mongo.Diagnostics");
            tracing.AddSource("CodeDesignPlus.Net.PubSub");
        }
    }

    private static void AddTraceRedisInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddRedisInstrumentation();
    }

    private static void AddTraceKafkaInstrumentation(this TracerProviderBuilder tracing, bool enable)
    {
        if (enable)
            tracing.AddConfluentKafkaInstrumentation();
    }
}
