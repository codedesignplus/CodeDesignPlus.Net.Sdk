namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a contract for initializing services during application startup.
/// </summary>
public interface IStartupServices
{
    /// <summary>
    /// Initializes the services required by the application.
    /// </summary>
    /// <param name="services">The collection of services to be initialized.</param>
    /// <param name="configuration">The configuration settings for the application.</param>
    void Initialize(IServiceCollection services, IConfiguration configuration);
}