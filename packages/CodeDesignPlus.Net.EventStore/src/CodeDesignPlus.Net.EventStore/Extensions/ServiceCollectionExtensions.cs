namespace CodeDesignPlus.Net.EventStore.Extensions;

/// <summary>
/// Provides extension methods for adding Event Store services to an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Event Store services and configurations to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> to retrieve the configuration section from.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the services or configuration is null.</exception>
    /// <exception cref="EventStoreException">Thrown when the required configuration section is missing.</exception>
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

        var eventStoreOptions = section.Get<EventStoreOptions>();

        services.AddEventSourcing(configuration, x =>
        {
            x.FrequencySnapshot = eventStoreOptions.FrequencySnapshot;
            x.MainName = eventStoreOptions.MainName;
            x.SnapshotSuffix = eventStoreOptions.SnapshotSuffix;
        });
        
        services.TryAddSingleton<IEventStoreConnection, EventStoreConnection>();
        services.TryAddSingleton<IEventStoreFactory, EventStoreFactory>();
        services.TryAddSingleton<IEventStore, EventStoreService>();
        services.TryAddSingleton<IEventSourcing, EventStoreService>();

        return services;
    }
}