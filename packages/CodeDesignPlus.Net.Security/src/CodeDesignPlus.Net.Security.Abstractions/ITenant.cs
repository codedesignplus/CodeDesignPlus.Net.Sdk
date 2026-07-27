using CodeDesignPlus.Net.Security.Abstractions.Models;
using CodeDesignPlus.Net.ValueObjects.Financial;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Interface that defines the methods to obtain the tenant information.
/// </summary>
public interface ITenant
{
    /// <summary>
    /// Loads the tenant information from <see cref="ITenantDirectory"/>. Safe to call repeatedly
    /// within the same scope, which is what a recurring job iterating over tenants needs.
    /// </summary>
    /// <param name="id">The identifier of the tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Return a <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exceptions.SecurityException">Thrown when the tenant cannot be resolved from any level.</exception>
    Task SetAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Gets a value indicating whether a tenant has been loaded into the current scope.
    /// </summary>
    bool IsLoaded { get; }
    /// <summary>
    /// Set the tenant information.
    /// </summary>
    /// <returns>Return true if the license is valid; otherwise, false.</returns>
    bool LicenseIsValid();
    /// <summary>
    /// Get the metadata value by key.
    /// </summary>
    /// <param name="key">The key to search in the metadata.</param>
    /// <returns>Return the value of the metadata.</returns>
    string GetMetadata(string key);
    /// <summary>
    /// Get the metadata value by key.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to return.</typeparam>
    /// <param name="key">The key to search in the metadata.</param>
    /// <returns>Return the value of the metadata.</returns>
    TValue GetMetadata<TValue>(string key);
    /// <summary>
    /// Get the country information.
    /// </summary>
    Country Country { get; }
    /// <summary>
    /// Get the state information.
    /// </summary>
    State State { get; }
    /// <summary>
    /// Get the city information.
    /// </summary>
    City City { get; }
    /// <summary>
    /// Get the locality information.
    /// </summary>
    Locality Locality { get; }
    /// <summary>
    /// Get the neighborhood information.
    /// </summary>
    Neighborhood Neighborhood { get; }
    /// <summary>
    /// Get the time zone.
    /// </summary>
    string TimeZone { get; }
    /// <summary>
    /// Get the currency.
    /// </summary>
    Currency Currency { get; }
    /// <summary>
    /// Get the metadata.
    /// </summary>
    Dictionary<string, string> Metadata { get; }
    /// <summary>
    /// Get the license modules purchased by the tenant.
    /// </summary>
    IReadOnlyList<LicenseModule> Modules { get; }
    /// <summary>
    /// Returns true if the tenant's license includes the module with the given ID.
    /// </summary>
    /// <param name="moduleId">The module identifier to check.</param>
    bool HasModule(Guid moduleId);
}
