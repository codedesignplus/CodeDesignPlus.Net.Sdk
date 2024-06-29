namespace CodeDesignPlus.Net.Event.Sourcing.Extensions;

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

        return services;
    }

}
