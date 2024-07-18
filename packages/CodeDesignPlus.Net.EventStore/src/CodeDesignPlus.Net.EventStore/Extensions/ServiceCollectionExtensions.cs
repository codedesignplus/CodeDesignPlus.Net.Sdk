namespace CodeDesignPlus.Net.EventStore.Extensions;

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
    public static IServiceCollection AddEventStore(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(EventStoreOptions.Section);

        if (!section.Exists())
            throw new EventStoreException($"The section {EventStoreOptions.Section} is required.");

        services
            .AddOptions<EventStoreOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IEventStoreConnection, EventStoreConnection>();
        services.AddSingleton<IEventStoreFactory, EventStoreFactory>();
        services.AddSingleton<IEventStoreService, EventStoreService>();
        services.AddSingleton<IEventSourcingService, EventStoreService>();

        return services;
    }

}
