using CodeDesignPlus.Net.File.Storage.Abstractions;
using CodeDesignPlus.Net.File.Storage.Exceptions;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Factories;

namespace CodeDesignPlus.Net.File.Storage.Extensions;

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
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(FileStorageOptions.Section);

        if (!section.Exists())
            throw new FileStorageException($"The section {FileStorageOptions.Section} is required.");

        services
            .AddOptions<FileStorageOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IFileStorageService, FileStorageService>();
        services.AddSingleton<IAzureBlobFactory, AzureBlobFactory>();
        services.AddSingleton<IAzureFlieFactory, AzureFileFactory>();
        services.AddSingleton<IAzureBlobProvider, AzureBlobProvider>();
        services.AddSingleton<IAzureFileProvider, AzureFileProvider>();
        services.AddSingleton<ILocalProvider, LocalProvider>();
        services.AddSingleton<IProvider, AzureBlobProvider>();
        services.AddSingleton<IProvider, AzureFileProvider>();
        services.AddSingleton<IProvider, LocalProvider>();

        return services;
    }

}
