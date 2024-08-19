namespace CodeDesignPlus.Net.Event.Sourcing.Extensions;

/// <summary>
/// Provides extension methods for setting up Event Sourcing services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Event Sourcing services to the specified <see cref="IServiceCollection"/> using the provided configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the services or configuration is null.</exception>
    /// <exception cref="EventSourcingException">Thrown if the required configuration section does not exist.</exception>
    public static IServiceCollection AddEventSourcing(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(EventSourcingOptions.Section);

        if (!section.Exists())
            throw new EventSourcingException($"The section {EventSourcingOptions.Section} is required.");

        services
            .AddOptions<EventSourcingOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddCore(configuration);

        return services;
    }

    /// <summary>
    /// Adds Event Sourcing services to the specified <see cref="IServiceCollection"/> using the provided configuration and setup options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <param name="setupOptions">An <see cref="Action{EventSourcingOptions}"/> to configure the provided <see cref="EventSourcingOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the services, configuration, or setupOptions is null.</exception>
    public static IServiceCollection AddEventSourcing(this IServiceCollection services, IConfiguration configuration, Action<EventSourcingOptions> setupOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(setupOptions);

        var section = new EventSourcingOptions();

        setupOptions(section);

        services
            .AddOptions<EventSourcingOptions>()
            .Configure(setupOptions)
            .ValidateDataAnnotations();

        services.AddCore(configuration);

        return services;
    }
}