namespace CodeDesignPlus.Net.Core.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core services to the specified <see cref="IServiceCollection"/> using the provided <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> used to configure the core services.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="CoreException">Thrown when the required configuration section is missing.</exception>
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(CoreOptions.Section);

        if (!section.Exists())
            throw new CoreException($"The section {CoreOptions.Section} is required.");

        services
            .AddOptions<CoreOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddSingleton<IDomainEventResolver, DomainEventResolverService>();
        
        services.AddStartups(configuration);

        return services;
    }

    /// <summary>
    /// Adds the services of the specified <see cref="IStartup"/> implementations to the specified <see cref="IServiceCollection"/> using the provided <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> used to configure the services.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    private static IServiceCollection AddStartups(this IServiceCollection services, IConfiguration configuration)
    {
        var startups = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.FullName.StartsWith("Castle") || !x.FullName.Contains("DynamicProxyGenAssembly"))
            .Where(x => typeof(IStartup).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => (IStartup)Activator.CreateInstance(x))
            .ToArray();

        foreach (var startup in startups)
        {
            startup.Initialize(services, configuration);
        }

        return services;
    }
}
