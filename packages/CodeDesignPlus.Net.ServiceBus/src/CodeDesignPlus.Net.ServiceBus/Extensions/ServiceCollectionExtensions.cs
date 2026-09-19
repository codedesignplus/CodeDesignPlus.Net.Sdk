namespace CodeDesignPlus.Net.ServiceBus.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.Net.ServiceBus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add support to Azure Service Bus as the transport of the pub/sub contract.
    /// </summary>
    /// <typeparam name="TAssembly">The type used to locate the assembly that contains the event handlers.</typeparam>
    /// <param name="services">A reference to this instance after the operation has completed.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="ServiceBusPubSubException">The <c>ServiceBus</c> section is missing from the configuration.</exception>
    public static IServiceCollection AddServiceBus<TAssembly>(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(ServiceBusOptions.Section);

        if (!section.Exists())
            throw new ServiceBusPubSubException($"The section {ServiceBusOptions.Section} is required.");

        services.AddOptions<ServiceBusOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<ServiceBusOptions>();

        if (!options.Enable)
            return services;

        services.AddPubSub(configuration, x =>
        {
            x.EnableDiagnostic = options.EnableDiagnostic;
            x.RegisterAutomaticHandlers = options.RegisterAutomaticHandlers;
            x.SecondsWaitQueue = options.SecondsWaitQueue;
            x.UseQueue = options.UseQueue;
        });

        services.TryAddSingleton<IServiceBusClientProvider, ServiceBusClientProvider>();
        services.TryAddSingleton<ISubscriptionNameResolver, SubscriptionNameResolver>();
        services.TryAddSingleton<IEntityProvisioner, EntityProvisioner>();
        services.TryAddSingleton<ServiceBusPubSubService>();
        services.TryAddSingleton<IMessage>(x => x.GetRequiredService<ServiceBusPubSubService>());
        services.TryAddSingleton<IServiceBusPubSub>(x => x.GetRequiredService<ServiceBusPubSubService>());

        if (options.RegisterHealthCheck)
        {
            // No se comprueba el namespace por separado. Los chequeos de entidad del paquete de health checks
            // exigen nombrar un topic y una suscripcion concretos, y cualquiera que se invente aqui no lo crea
            // nadie: la sonda quedaria en rojo para siempre. La comprobacion de abajo ya prueba lo mismo por via
            // indirecta, porque no se puede abrir una suscripcion sin alcanzar el namespace.
            //
            // Sin este, un pod cuyos consumidores no llegaron a suscribirse pasaria igualmente la readinessProbe
            // y entraria en rotacion sin consumir nada.
            services.AddHealthChecks().AddCheck<SubscriptionReadinessHealthCheck>("ServiceBus-Consumers", tags: ["ready"]);
        }

        return services;
    }
}
