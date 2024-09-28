namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;

/// <summary>
/// Interface for creating and managing Azure File Storage clients.
/// </summary>
public interface IAzureFileFactory
{
    /// <summary>
    /// Gets the file storage options.
    /// </summary>
    FileStorageOptions Options { get; }

    /// <summary>
    /// Gets the user context.
    /// </summary>
    IUserContext UserContext { get; }

    /// <summary>
    /// Gets the Share service client.
    /// </summary>
    ShareServiceClient Client { get; }

    /// <summary>
    /// Creates and initializes the Share service client if it is not already created.
    /// </summary>
    /// <returns>The current instance of <see cref="IAzureFileFactory"/>.</returns>
    IAzureFileFactory Create();

    /// <summary>
    /// Gets the Share client for the current tenant.
    /// </summary>
    /// <returns>The Share client.</returns>
    ShareClient GetContainerClient();
}