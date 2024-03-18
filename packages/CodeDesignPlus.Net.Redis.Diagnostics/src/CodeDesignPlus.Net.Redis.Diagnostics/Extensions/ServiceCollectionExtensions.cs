using CodeDesignPlus.Net.Redis.Diagnostics.Abstractions;
using CodeDesignPlus.Net.Redis.Diagnostics.Exceptions;
using CodeDesignPlus.Net.Redis.Diagnostics.Options;
using CodeDesignPlus.Net.Redis.Diagnostics.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Redis.Diagnostics.Extensions;

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
    public static IServiceCollection AddRedisDiagnostics(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(RedisDiagnosticsOptions.Section);

        if (!section.Exists())
            throw new RedisDiagnosticsException($"The section {RedisDiagnosticsOptions.Section} is required.");

        services
            .AddOptions<RedisDiagnosticsOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IRedisDiagnosticsService, RedisDiagnosticsService>();

        return services;
    }

}
