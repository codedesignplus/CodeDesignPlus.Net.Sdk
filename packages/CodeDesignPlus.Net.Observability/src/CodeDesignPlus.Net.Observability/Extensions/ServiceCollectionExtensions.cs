using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Observability.Abstractions.Options;
using CodeDesignPlus.Net.Observability.Exceptions;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CodeDesignPlus.Net.Observability.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        var observabilitySection = configuration.GetSection(ObservabilityOptions.Section);
        var coreSection = configuration.GetSection(CoreOptions.Section);

        if (!observabilitySection.Exists())
            throw new ObservabilityException($"The section {ObservabilityOptions.Section} is required.");

        var observabilityOptions = observabilitySection.Get<ObservabilityOptions>();
        var coreOptions = coreSection.Get<CoreOptions>();

        services
            .AddOptions<ObservabilityOptions>()
            .Bind(observabilitySection)
            .ValidateDataAnnotations();

        var otel = services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(serviceName: coreOptions.AppName, serviceVersion: coreOptions.Version);
            });

        if (observabilityOptions.Metrics.Enable)
            otel.WithMetrics(x =>
            {
                x.AddOtlpExporter(x =>
                {
                    x.Endpoint = observabilityOptions.ServerOtel;
                    x.Protocol = OtlpExportProtocol.Grpc;
                });

                if (observabilityOptions.Metrics.AspNetCore)
                    x.AddAspNetCoreInstrumentation();

                if (environment.IsDevelopment())
                    x.AddConsoleExporter();
            });

        if (observabilityOptions.Trace.Enable)
            otel.WithTracing(tracing =>
            {
                tracing.AddOtlpExporter(x =>
                {
                    x.Endpoint = observabilityOptions.ServerOtel;
                    x.Protocol = OtlpExportProtocol.Grpc;
                });

                if (observabilityOptions.Trace.AspNetCore)
                {
                    tracing.AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = (httpContext) => httpContext.Request.Path.StartsWithSegments("/api");
                    });

                    tracing.AddHttpClientInstrumentation();
                }

                if (observabilityOptions.Trace.GrpcClient)
                {
                    tracing.AddGrpcClientInstrumentation();
                    tracing.AddHttpClientInstrumentation();
                }

                if (observabilityOptions.Trace.SqlClient)
                    tracing.AddSqlClientInstrumentation();

                if (observabilityOptions.Trace.CodeDesignPlusSdk)
                {
                    tracing.AddSource("CodeDesignPlus.Net.Mongo.Diagnostics");
                    tracing.AddSource("CodeDesignPlus.Net.PubSub");
                }

                if (observabilityOptions.Trace.Redis)
                    tracing.AddRedisInstrumentation();

                if (observabilityOptions.Trace.Kafka)
                    tracing.AddConfluentKafkaInstrumentation();

                if (environment.IsDevelopment())
                    tracing.AddConsoleExporter();
            });




        return services;
    }
}
