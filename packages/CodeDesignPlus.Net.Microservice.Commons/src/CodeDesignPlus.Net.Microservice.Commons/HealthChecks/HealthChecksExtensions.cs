using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeDesignPlus.Net.Microservice.Commons.HealthChecks;

/// <summary>
/// Extension methods for setting up HealthChecks services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class HealthChecksExtensions
{
    /// <summary>
    /// Adds HealthChecks services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = HealthCheckServiceCollectionExtensions.AddHealthChecks(services);

        builder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        return services;
    }

    /// <summary>
    /// Adds HealthChecks services to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to add the services to.</param>
    public static void UseHealthChecks(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        HealthCheckEndpointRouteBuilderExtensions.MapHealthChecks(app, "/health/ready", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("ready")
        });

        HealthCheckEndpointRouteBuilderExtensions.MapHealthChecks(app, "/health/live", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("live")
        });
    }
}
