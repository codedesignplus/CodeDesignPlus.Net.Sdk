namespace CodeDesignPlus.Net.File.Storage.Extensions;

/// <summary>
/// Provides extension methods for registering file storage services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds file storage services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration to bind the options from.</param>
    /// <returns>The service collection with the file storage services added.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="FileStorageException">
    /// Thrown when the required configuration section is missing.
    /// </exception>
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(FileStorageOptions.Section);

        if (!section.Exists())
            throw new FileStorageException($"The section {FileStorageOptions.Section} is required.");

        services
            .AddOptions<FileStorageOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IFileStorageService, FileStorageService>();
        services.AddSingleton<IAzureBlobFactory, AzureBlobFactory>();
        services.AddSingleton<IAzureFileFactory, AzureFileFactory>();
        services.AddSingleton<IAzureBlobProvider, AzureBlobProvider>();
        services.AddSingleton<IAzureFileProvider, AzureFileProvider>();
        services.AddSingleton<ILocalProvider, LocalProvider>();
        services.AddSingleton<IProvider, AzureBlobProvider>();
        services.AddSingleton<IProvider, AzureFileProvider>();
        services.AddSingleton<IProvider, LocalProvider>();

        return services;
    }
}