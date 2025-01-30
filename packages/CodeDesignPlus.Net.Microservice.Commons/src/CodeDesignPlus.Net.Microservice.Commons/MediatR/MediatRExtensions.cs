using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.MediatR;

/// <summary>
/// Extensions for configuring MediatR in the service container.
/// </summary>
public static class MediatRExtensions
{
    /// <summary>
    /// Adds and configures MediatR in the service container.
    /// </summary>
    /// <typeparam name="TApplication">The application type to load the assembly from.</typeparam>
    /// <param name="services">The service container.</param>
    /// <returns>The service container with MediatR configured.</returns>
    public static IServiceCollection AddMediatR<TApplication>(this IServiceCollection services) 
        where TApplication : IStartup
    {
        var assembly = typeof(TApplication).Assembly;

        services.AddMediatR(config => {
            config.RegisterServicesFromAssemblies(assembly);

            config.AddOpenBehavior(typeof(ValidationPipeline<,>));
        });

        return services;
    }
}