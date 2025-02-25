using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;

/// <summary>
/// Class extension for the resources.
/// </summary>
public static class ResourcesExtensions
{
    /// <summary>
    /// Add the resources to the application.
    /// </summary>
    /// <typeparam name="TProgram">The program class of the microservice.</typeparam>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="configuration">The configuration of the microservice.</param>
    /// <returns>The collection of service descriptors.</returns>
    public static IServiceCollection AddResources<TProgram>(this IServiceCollection services, IConfiguration configuration) where TProgram : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetRequiredSection(ResourcesOptions.Section);

        services
            .AddOptions<ResourcesOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<ResourcesOptions>();

        if (!options.Enable)
            return services;

        
        services.AddSingleton<ResourceHealtCheck>();

        services
            .AddHealthChecks()
            .AddCheck<ResourceHealtCheck>("Resource", tags: ["ready"]);

        services.AddHostedService<RegisterResourcesBackgroundService<TProgram>>();

        services.AddGrpcClient<Services.gRpc.Service.ServiceClient>(o =>
        {
            o.Address = options.Server;
        });

        return services;
    }
}
