using CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;
using CodeDesignPlus.Net.gRpc.Clients.Services.Users;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;

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

        var section = configuration.GetRequiredSection(GrpcClientsOptions.Section);

        services
            .AddOptions<GrpcClientsOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var options = section.Get<GrpcClientsOptions>();

        if (!string.IsNullOrEmpty(options!.Payment))
        {
            services.AddGrpcClient<Payment.PaymentClient>(o =>
            {
                o.Address = new Uri(options.Payment);
            });

            services.AddScoped<IPaymentGrpc, PaymentService>();
        }

        if (!string.IsNullOrEmpty(options!.User))
        {
            services.AddGrpcClient<Services.User.Users.UsersClient>(o =>
            {
                o.Address = new Uri(options.User);
            });

            services.AddScoped<IUserGrpc, UserService>();
        }

        if (!string.IsNullOrEmpty(options!.Tenant))
        {
            services.AddGrpcClient<Services.Tenant.Tenant.TenantClient>(o =>
            {
                o.Address = new Uri(options.Tenant);
            });

            services.AddScoped<ITenantGrpc, TenantService>();
        }

        return services;
    }

}
