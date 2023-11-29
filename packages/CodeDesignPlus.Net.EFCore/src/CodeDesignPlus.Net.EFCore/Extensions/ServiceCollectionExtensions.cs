using System.Reflection;
using CodeDesignPlus.Net.EFCore.Exceptions;
using CodeDesignPlus.Net.EFCore.Abstractions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.EFCore.Extensions;

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
    public static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(EFCoreOptions.Section);

        if (!section.Exists())
            throw new EFCoreException($"The section {EFCoreOptions.Section} is required.");

        services
            .AddOptions<EFCoreOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        return services;
    }

    /// <summary>
    /// Gets all repositories and registers them in the.net core dependency container
    /// </summary>
    /// <typeparam name="TKey">Type of data that will identify the record</typeparam>
    /// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
    /// <typeparam name="TContext">Represents a session with the database and can be used to query and save instances of your entities</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    public static void AddRepositories<TKey, TUserKey, TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        var assembly = typeof(TContext).GetTypeInfo().Assembly;

        var @types = assembly.GetTypes().Where(x => !x.IsNested && !x.IsInterface && typeof(IRepositoryBase<TKey, TUserKey>).IsAssignableFrom(x));

        foreach (var type in @types)
        {
            var @interface = type.GetInterface($"I{type.Name}", false);

            services.AddTransient(@interface, type);
        }
    }
}
