
using Microsoft.Extensions.Logging;
using CodeDesignPlus.Net.Cache.Abstractions;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Tenant service to manage the tenant information. 
/// </summary>
public class Tenant : ITenant
{
    private Models.Tenant tenant;
    private readonly ILogger<Tenant> logger;
    private readonly ICacheManager cacheManager;

    /// <summary>
    /// Create a new instance of <see cref="Tenant"/>.
    /// </summary>
    /// <param name="logger">The logger service.</param>
    /// <param name="cacheManager">The cache manager service.</param>
    public Tenant(ILogger<Tenant> logger, ICacheManager cacheManager)
    {
        this.logger = logger;
        this.cacheManager = cacheManager;

        this.logger.LogInformation("TenantService initialized");
    }

    /// <summary>
    /// Set the tenant information.
    /// </summary>
    /// <param name="id">The identifier of the tenant.</param>
    /// <returns>Return a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SetTenantAsync(Guid id)
    {
        var exist = await cacheManager.ExistsAsync($"Tenant:{id}");

        if (!exist)
            throw new SecurityException("The tenant specified does not exist at the level of the cache.");

        this.tenant = await cacheManager.GetAsync<Models.Tenant>($"Tenant:{id}");

        this.logger.LogDebug("Tenant loaded: {TenantId}", id);
    }

    /// <summary>
    /// Set the tenant information.
    /// </summary>
    /// <returns>Return true if the license is valid; otherwise, false.</returns>
    public bool LicenseIsValid()
    {
        var now = NodaTime.SystemClock.Instance.GetCurrentInstant();

        var isValid = this.tenant.License.StartDate < now && this.tenant.License.ExpirationDate > now;

        this.logger.LogDebug("The license with id {LicenseId} is valid: {IsValid}, StartDate: {StartDate}, ExpirationDate: {ExpirationDate}", this.tenant.License.Id, isValid, this.tenant.License.StartDate, this.tenant.License.ExpirationDate);

        return isValid;
    }

    /// <summary>
    /// Get the metadata value by key.
    /// </summary>
    /// <param name="key">The key to search in the metadata.</param>
    /// <returns>Return the value of the metadata.</returns>
    public string GetMetadata(string key)
    {
        return this.tenant.Metadata[key];
    }

    /// <summary>
    /// Get the metadata value by key.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to return.</typeparam>
    /// <param name="key">The key to search in the metadata.</param>
    /// <returns>Return the value of the metadata.</returns>
    public TValue GetMetadata<TValue>(string key)
    {
        var value = this.tenant.Metadata[key] ?? throw new KeyNotFoundException($"The key {key} does not exist in the metadata.");
        
        return (TValue)Convert.ChangeType(value, typeof(TValue));
    }

    /// <summary>
    /// Get the country information.
    /// </summary>
    public Models.Country Country => this.tenant.Location.Country;
    /// <summary>
    /// Get the state information.
    /// </summary>
    public Models.State State => this.tenant.Location.State;
    /// <summary>
    /// Get the city information.
    /// </summary>
    public Models.City City => this.tenant.Location.City;
    /// <summary>
    /// Get the locality information.
    /// </summary>
    public Models.Locality Locality => this.tenant.Location.Locality;
    /// <summary>
    /// Get the neighborhood information.
    /// </summary>
    public Models.Neighborhood Neighborhood => this.tenant.Location.Neighborhood;
    /// <summary>
    /// Get the time zone.
    /// </summary>
    public string TimeZone => this.tenant.Location.City.TimeZone ?? this.tenant.Location.Country.TimeZone;
    /// <summary>
    /// Get the currency.
    /// </summary>
    public Models.Currency Currency => this.tenant.Location.Country.Currency;
    /// <summary>
    /// Get the metadata.
    /// </summary>
    public Dictionary<string, string> Metadata => this.tenant.Metadata;
}
