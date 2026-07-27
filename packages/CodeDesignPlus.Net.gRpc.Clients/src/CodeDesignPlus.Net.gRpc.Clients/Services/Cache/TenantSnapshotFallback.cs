using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.ValueObjects.Financial;
using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;
using TenantProto = CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using VoLocation = CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Cache;

/// <summary>
/// Builds a tenant snapshot straight from ms-tenants, used when the shared cache cannot serve it.
/// ms-tenants owns the whole snapshot — location, currency, license and modules — so no other
/// service needs to be queried.
/// </summary>
public class TenantSnapshotFallback(ITenantGrpc tenantGrpc, ILogger<TenantSnapshotFallback> logger) : ITenantSnapshotFallback
{
    /// <inheritdoc/>
    public async Task<Models.Tenant> GetAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("Tenant {TenantId} is not available in the shared cache; falling back to ms-tenants", tenantId);

        var response = await tenantGrpc.GetTenantByIdAsync(
            new TenantProto.GetTenantRequest { Id = tenantId.ToString() }, cancellationToken);

        if (response is null)
            return null;

        return new Models.Tenant
        {
            Id = tenantId,
            Name = response.Name,
            Domain = string.IsNullOrEmpty(response.Domain) ? null : new Uri(response.Domain),
            License = BuildLicense(response.License),
            Location = BuildLocation(response.Location),
            Metadata = response.License?.Metadata?.ToDictionary(k => k.Key, v => v.Value) ?? []
        };
    }

    private static Models.License BuildLicense(TenantProto.License proto)
    {
        if (proto is null)
            return new Models.License();

        return new Models.License
        {
            Id = Guid.TryParse(proto.Id, out var id) ? id : Guid.Empty,
            Name = proto.Name,
            StartDate = ParseInstant(proto.StartDate, NodaTime.Instant.MinValue),
            ExpirationDate = ParseInstant(proto.EndDate, NodaTime.Instant.MaxValue),
            Modules = [.. proto.Modules.Select(module => new Models.LicenseModule
            {
                Id = Guid.TryParse(module.Id, out var moduleId) ? moduleId : Guid.Empty,
                Name = module.Name
            })],
            Metadata = proto.Metadata?.ToDictionary(k => k.Key, v => v.Value) ?? []
        };
    }

    private static NodaTime.Instant ParseInstant(string value, NodaTime.Instant fallback)
    {
        if (string.IsNullOrEmpty(value))
            return fallback;

        return DateTimeOffset.TryParse(value, out var parsed)
            ? NodaTime.Instant.FromDateTimeOffset(parsed)
            : fallback;
    }

    private static VoLocation.Location BuildLocation(TenantProto.Location proto)
    {
        if (proto is null)
            throw new InvalidOperationException("Tenant location is required.");

        var currency = Currency.Create(
            Guid.TryParse(proto.Country?.Currency?.Id, out var cid) ? cid : Guid.NewGuid(),
            proto.Country?.Currency?.Name ?? string.Empty,
            proto.Country?.Currency?.Code ?? "XXX",
            proto.Country?.Currency?.Symbol ?? string.Empty,
            (short)(proto.Country?.Currency?.DecimalDigits ?? 2),
            (short)(proto.Country?.Currency?.NumericCode ?? 0));

        var country = VoLocation.Country.Create(
            Guid.TryParse(proto.Country?.Id, out var countryId) ? countryId : Guid.NewGuid(),
            proto.Country?.Name ?? string.Empty,
            proto.Country?.Alpha2 ?? "XX",
            proto.Country?.Alpha3 ?? "XXX",
            (ushort)(proto.Country?.Code ?? 1),
            proto.Country?.PhoneCode ?? "+1",
            proto.Country?.Timezone ?? "UTC",
            currency);

        var state = VoLocation.State.Create(
            Guid.TryParse(proto.State?.Id, out var stateId) ? stateId : Guid.NewGuid(),
            proto.State?.Name ?? string.Empty,
            proto.State?.Code ?? "XX");

        var cityTimezone = string.IsNullOrEmpty(proto.City?.Timezone) ? null : proto.City.Timezone;
        var city = VoLocation.City.Create(
            Guid.TryParse(proto.City?.Id, out var cityId) ? cityId : Guid.NewGuid(),
            proto.City?.Name ?? string.Empty,
            cityTimezone);

        var locality = VoLocation.Locality.Create(
            Guid.TryParse(proto.Locality?.Id, out var localityId) ? localityId : Guid.NewGuid(),
            proto.Locality?.Name ?? string.Empty);

        var neighborhood = VoLocation.Neighborhood.Create(
            Guid.TryParse(proto.Neighborhood?.Id, out var neighborhoodId) ? neighborhoodId : Guid.NewGuid(),
            proto.Neighborhood?.Name ?? string.Empty);

        return VoLocation.Location.Create(country, state, city, locality, neighborhood,
            proto.Address ?? string.Empty,
            proto.PostalCode ?? string.Empty);
    }
}
