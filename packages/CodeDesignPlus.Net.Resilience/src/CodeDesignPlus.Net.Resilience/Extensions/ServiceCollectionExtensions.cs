using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace CodeDesignPlus.Net.Resilience.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResilience(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter("Polly"));
        services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddSource("Polly"));

        return services;
    }
}
