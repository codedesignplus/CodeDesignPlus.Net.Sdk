using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Extensions;

/// <summary>
/// Extension methods for configuring gRPC services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers gRPC interceptors in the service collection.
    /// </summary>
    /// <param name="services">Service collection to add the interceptors to.</param>
    /// <returns>Return the updated service collection.</returns>
    public static IServiceCollection AddGrpcInterceptors(this IServiceCollection services)
    {
        services.AddSingleton<ErrorInterceptor>();

        return services;
    }

}
