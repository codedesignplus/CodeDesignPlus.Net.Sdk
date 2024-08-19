namespace CodeDesignPlus.Net.EventStore.PubSub.Extensions;

/// <summary>
/// Provides extension methods for registering EventStore Pub/Sub services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds EventStore Pub/Sub services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration to bind the options from.</param>
    /// <returns>The service collection with the EventStore Pub/Sub services added.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="EventStorePubSubException">
    /// Thrown when the required configuration section is missing.
    /// </exception>
    public static IServiceCollection AddEventStorePubSub(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(EventStorePubSubOptions.Section);

        if (!section.Exists())
            throw new EventStorePubSubException($"The section {EventStorePubSubOptions.Section} is required.");

        services
            .AddOptions<EventStorePubSubOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<EventStorePubSubOptions>();

        if (options.Enabled)
        {
            services.AddEventStore(configuration);
            services.AddPubSub(configuration, x => {
                x.EnableDiagnostic = options.EnableDiagnostic;
                x.RegisterAutomaticHandlers = options.RegisterAutomaticHandlers;
                x.SecondsWaitQueue = options.SecondsWaitQueue;
                x.UseQueue = options.UseQueue;
            });
            services.TryAddSingleton<IMessage, EventStorePubSubService>();
            services.TryAddSingleton<IEventStorePubSubService, EventStorePubSubService>();
        }

        return services;
    }
}