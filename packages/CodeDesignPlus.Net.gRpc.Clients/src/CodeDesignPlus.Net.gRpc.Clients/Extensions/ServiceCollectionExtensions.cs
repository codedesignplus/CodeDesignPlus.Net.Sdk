using CodeDesignPlus.Net.gRpc.Clients.Abstractions;
using CodeDesignPlus.Net.gRpc.Clients.Exceptions;
using CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;
using CodeDesignPlus.Net.gRpc.Clients.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

namespace CodeDesignPlus.Net.gRpc.Clients.Extensions;

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
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(GrpcClientsOptions.Section);

        if (!section.Exists())
            throw new GrpcClientsException($"The section {GrpcClientsOptions.Section} is required.");

        services
            .AddOptions<GrpcClientsOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var options = section.Get<GrpcClientsOptions>();

        if (!string.IsNullOrEmpty(options.PaymentUrl))
        {
            services.AddGrpcClient<CodeDesignPlus.Net.gRpc.Clients.Services.Payment.Payment.PaymentClient>(o =>
            {
                o.Address = new Uri(options.PaymentUrl);
            });

            services.AddScoped<IPayment, PaymentService>();
        }

        return services;
    }

}
