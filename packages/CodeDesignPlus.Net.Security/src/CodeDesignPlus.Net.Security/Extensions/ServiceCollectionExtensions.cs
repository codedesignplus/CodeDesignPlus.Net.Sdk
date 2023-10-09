using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.Security.Exceptions;
using CodeDesignPlus.Net.Security.Options;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Security.Extensions;

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
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(SecurityOptions.Section);

        if (!section.Exists())
            throw new SecurityException($"The section {SecurityOptions.Section} is required.");

        services
            .AddOptions<SecurityOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<ISecurityService, SecurityService>();

        return services;
    }

}
