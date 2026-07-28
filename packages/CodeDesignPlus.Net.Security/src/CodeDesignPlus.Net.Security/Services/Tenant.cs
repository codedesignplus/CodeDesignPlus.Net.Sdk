using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;
using CodeDesignPlus.Net.ValueObjects.Location;
using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Holds the tenant loaded into the current scope. Resolving the snapshot is the responsibility of
/// <see cref="ITenantDirectory"/>.
/// </summary>
/// <param name="logger">The logger service.</param>
/// <param name="directory">The tenant directory.</param>
public class Tenant(ILogger<Tenant> logger, ITenantDirectory directory) : ITenant
{
    private Models.Tenant tenant;

    /// <inheritdoc/>
    public bool IsLoaded => this.tenant is not null;

    // A diferencia del resto de propiedades, esta no llama a EnsureLoaded: su unico consumidor es la
    // telemetria, y que una traza salga sin nombre es preferible a que tumbe la peticion.
    /// <inheritdoc/>
    public string Name => this.tenant?.Name;

    /// <inheritdoc/>
    public async Task SetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        this.tenant = await directory.GetSnapshotAsync(id, cancellationToken)
            ?? throw new SecurityException($"The tenant {id} could not be resolved.");

        logger.LogDebug("Tenant loaded: {TenantId}", id);
    }

    /// <inheritdoc/>
    public bool LicenseIsValid()
    {
        this.EnsureLoaded();

        var now = NodaTime.SystemClock.Instance.GetCurrentInstant();

        var isValid = this.tenant.License.StartDate < now && this.tenant.License.ExpirationDate > now;

        logger.LogDebug("The license with id {LicenseId} is valid: {IsValid}, StartDate: {StartDate}, ExpirationDate: {ExpirationDate}", this.tenant.License.Id, isValid, this.tenant.License.StartDate, this.tenant.License.ExpirationDate);

        return isValid;
    }

    /// <inheritdoc/>
    public string GetMetadata(string key)
    {
        this.EnsureLoaded();

        return this.tenant.Metadata[key];
    }

    /// <inheritdoc/>
    public TValue GetMetadata<TValue>(string key)
    {
        this.EnsureLoaded();

        var value = this.tenant.Metadata[key] ?? throw new KeyNotFoundException($"The key {key} does not exist in the metadata.");

        return (TValue)Convert.ChangeType(value, typeof(TValue));
    }

    /// <inheritdoc/>
    public Country Country => this.GetLocation().Country;

    /// <inheritdoc/>
    public State State => this.GetLocation().State;

    /// <inheritdoc/>
    public City City => this.GetLocation().City;

    /// <inheritdoc/>
    public Locality Locality => this.GetLocation().Locality;

    /// <inheritdoc/>
    public Neighborhood Neighborhood => this.GetLocation().Neighborhood;

    /// <inheritdoc/>
    public string TimeZone => this.GetLocation().City.Timezone ?? this.GetLocation().Country.Timezone;

    /// <inheritdoc/>
    public Currency Currency => this.GetLocation().Country.Currency;

    /// <inheritdoc/>
    public Dictionary<string, string> Metadata
    {
        get
        {
            this.EnsureLoaded();

            return this.tenant.Metadata;
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<Models.LicenseModule> Modules
    {
        get
        {
            this.EnsureLoaded();

            return this.tenant.License.Modules;
        }
    }

    /// <inheritdoc/>
    public bool HasModule(Guid moduleId) => this.Modules.Any(module => module.Id == moduleId);

    private Location GetLocation()
    {
        this.EnsureLoaded();

        return this.tenant.Location;
    }

    private void EnsureLoaded()
    {
        if (this.tenant is null)
            throw new SecurityException("No tenant has been loaded in the current scope. Call SetAsync first.");
    }
}
