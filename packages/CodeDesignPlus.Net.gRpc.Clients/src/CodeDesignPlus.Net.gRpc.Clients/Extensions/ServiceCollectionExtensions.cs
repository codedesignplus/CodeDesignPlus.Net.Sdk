using CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;
using CodeDesignPlus.Net.gRpc.Clients.Services.Users;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;
using CodeDesignPlus.Net.gRpc.Clients.Services.Currencies;
using CodeDesignPlus.Net.gRpc.Clients.Services.Countries;
using CodeDesignPlus.Net.gRpc.Clients.Services.Memory;
using CodeDesignPlus.Net.gRpc.Clients.Services.Licenses;
using CodeDesignPlus.Net.gRpc.Clients.Services.Modules;
using LicenseGrpc = CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc.LicenseService;
using ModuleGrpc = CodeDesignPlus.Net.Microservice.Modules.gRpc.Module;
using CodeDesignPlus.Net.gRpc.Clients.Services.Cache;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.gRpc.Clients.Services.Emails;

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

        if (!string.IsNullOrEmpty(options!.Notification))
        {
            services.AddGrpcClient<Services.Notification.Notifier.NotifierClient>(o =>
            {
                o.Address = new Uri(options.Notification);
            });

            services.AddSingleton<INotificationGrpc, NotificationService>();
        }

        if (!string.IsNullOrEmpty(options!.Location))
        {
            services.AddGrpcClient<CurrencyService.CurrencyServiceClient>(o =>
            {
                o.Address = new Uri(options.Location);
            });

            services.AddGrpcClient<CountryService.CountryServiceClient>(o =>
            {
                o.Address = new Uri(options.Location);
            });

            services.AddSingleton<ICurrencyGrpc, CurrenciesService>();
            services.AddSingleton<ICountryGrpc, CountriesService>();
            services.AddSingleton<IMemoryService<ValueObjects.Financial.Currency>, MemoryService<ValueObjects.Financial.Currency>>();
            services.AddSingleton<IMemoryService<ValueObjects.Location.Country>, MemoryService<ValueObjects.Location.Country>>();

        }

        if (!string.IsNullOrEmpty(options!.License))
        {
            services.AddGrpcClient<LicenseGrpc.LicenseServiceClient>(o =>
            {
                o.Address = new Uri(options.License);
            });

            services.AddScoped<ILicenseGrpc, LicenseService>();
        }

        if (!string.IsNullOrEmpty(options!.Module))
        {
            services.AddGrpcClient<ModuleGrpc.ModuleClient>(o =>
            {
                o.Address = new Uri(options.Module);
            });

            services.AddScoped<IModuleGrpc, ModuleClientService>();
        }

        if (!string.IsNullOrEmpty(options!.Email))
        {
            services.AddGrpcClient<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>(o =>
            {
                o.Address = new Uri(options.Email);
            });

            services.AddScoped<IEmailGrpc, EmailService>();
        }

        if (!string.IsNullOrEmpty(options!.Tenant) && !string.IsNullOrEmpty(options!.License))
            services.AddScoped<ITenantCacheLoader, TenantCacheLoader>();

        return services;
    }

}
