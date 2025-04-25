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

        services.AddScoped<IFileStorage, FileStorageService>();
        services.AddScoped<IAzureBlobFactory, AzureBlobFactory>();
        services.AddScoped<IAzureFileFactory, AzureFileFactory>();
        services.AddScoped<IAzureBlobProvider, AzureBlobProvider>();
        services.AddScoped<IAzureFileProvider, AzureFileProvider>();
        services.AddScoped<ILocalProvider, LocalProvider>();
        services.AddScoped<IProvider, AzureBlobProvider>();
        services.AddScoped<IProvider, AzureFileProvider>();
        services.AddScoped<IProvider, LocalProvider>();

        return services;
    }
}