using CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Interface that defines the methods to obtain the tenant information.
/// </summary>
public interface ITenant
{
    /// <summary>
    /// Set the tenant information.
    /// </summary>
    /// <param name="id">The identifier of the tenant.</param>
    /// <returns>Return a <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetTenantAsync(Guid id);
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
}
