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
using CodeDesignPlus.Net.gRpc.Clients.Services.FileStorage;

namespace CodeDesignPlus.Net.gRpc.Clients.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.EFCore
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Tamaño máximo de un mensaje enviado a ms-filestorage: 16 MB, el mismo que acepta el servidor.
    /// </summary>
    private const int FileStorageMaxMessageSize = 16 * 1024 * 1024;

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

            // Los roles por copropiedad los publica ms-users en la cache compartida; este es el ultimo
            // recurso cuando esa cache no puede servirlos.
            services.AddScoped<IRoleSnapshotFallback, RoleSnapshotFallback>();
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

            services.AddGrpcClient<Services.Notification.LiveChannel.LiveChannelClient>(o =>
            {
                o.Address = new Uri(options.Notification);
            });

            services.AddGrpcClient<Services.Notification.Inbox.InboxClient>(o =>
            {
                o.Address = new Uri(options.Notification);
            });

            // Singleton como el Notifier: cada uno mantiene su stream abierto y su cola en memoria, y un
            // cliente scoped abriria un stream nuevo por peticion.
            services.AddSingleton<ILiveChannelGrpc, LiveChannelService>();
            services.AddSingleton<IInboxGrpc, InboxService>();
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

        if (!string.IsNullOrEmpty(options!.FileStorage))
        {
            services
                .AddGrpcClient<Services.FileStorage.Files.FilesClient>(o =>
                {
                    o.Address = new Uri(options.FileStorage);
                })
                // 16 MB, igual que el servidor. El frontend deja subir hasta 10 MB por archivo y un archivo
                // generado en el backend puede pesar lo mismo; con los 4 MB que gRPC trae por defecto en la
                // recepción del servidor, la llamada fallaria con ResourceExhausted. El límite de envío del
                // cliente se fija igual para que el tope sea uno solo y explícito en los dos extremos.
                .ConfigureChannel(channel => channel.MaxSendMessageSize = FileStorageMaxMessageSize);

            services.AddScoped<IFileStorageGrpc, FileStorageGrpcService>();
        }

        // El snapshot lo publica ms-tenants, que es dueño de todo el contenido: alcanza con su
        // endpoint para reconstruirlo cuando el cache compartido no puede servirlo.
        if (!string.IsNullOrEmpty(options!.Tenant))
            services.AddScoped<ITenantSnapshotFallback, TenantSnapshotFallback>();

        return services;
    }

}
