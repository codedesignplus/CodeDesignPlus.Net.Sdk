using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.ValueObjects.Financial;
using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;
using TenantProto = CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using LicenseProto = CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc;
using VoLocation = CodeDesignPlus.Net.ValueObjects.Location;
using GetTenantLicenseResponse = CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc.GetTenantLicenseResponse;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Cache;

/// <summary>
/// Hydrates the tenant cache on first access by calling ms-tenants (location + basic data)
/// and ms-licenses (license snapshot with modules).
/// </summary>
public class TenantCacheLoader(
    ITenantGrpc tenantGrpc,
    ILicenseGrpc licenseGrpc,
    ICacheManager cacheManager,
    ILogger<TenantCacheLoader> logger) : ITenantCacheLoader
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(1);

    /// <inheritdoc/>
    public async Task EnsureCachedAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"Tenant:{tenantId}";

        if (await cacheManager.ExistsAsync(cacheKey))
            return;

        logger.LogDebug("Tenant {TenantId} not in cache — loading from gRPC services", tenantId);

        var tenantResponse = await tenantGrpc.GetTenantByIdAsync(
            new TenantProto.GetTenantRequest { Id = tenantId.ToString() }, cancellationToken);

        var licenseResponse = await licenseGrpc.GetTenantLicenseAsync(tenantId, cancellationToken);

        var tenant = BuildTenant(tenantId, tenantResponse, licenseResponse);

        await cacheManager.SetAsync(cacheKey, tenant, CacheTtl);

        logger.LogInformation("Tenant {TenantId} cached successfully", tenantId);
    }

    private static Models.Tenant BuildTenant(
        Guid tenantId,
        TenantProto.GetTenantResponse tenantResponse,
        GetTenantLicenseResponse licenseResponse)
    {
        var location = BuildLocation(tenantResponse.Location);
        var license = BuildLicense(tenantResponse.License, licenseResponse);

        return new Models.Tenant
        {
            Id = tenantId,
            Name = tenantResponse.Name,
            Domain = string.IsNullOrEmpty(tenantResponse.Domain) ? null : new Uri(tenantResponse.Domain),
            License = license,
            Location = location,
            Metadata = tenantResponse.License?.Metadata?.ToDictionary(k => k.Key, v => v.Value) ?? []
        };
    }

    private static Models.License BuildLicense(
        TenantProto.License? proto,
        GetTenantLicenseResponse licenseSnapshot)
    {
        var license = new Models.License();

        if (!string.IsNullOrEmpty(licenseSnapshot.LicenseId))
        {
            license.Id = Guid.TryParse(licenseSnapshot.LicenseId, out var lid) ? lid : Guid.Empty;
            license.Name = licenseSnapshot.Name;
            license.StartDate = licenseSnapshot.StartDate is not null
                ? NodaTime.Instant.FromUnixTimeSeconds(licenseSnapshot.StartDate.Seconds)
                : NodaTime.Instant.MinValue;
            license.ExpirationDate = licenseSnapshot.EndDate is not null
                ? NodaTime.Instant.FromUnixTimeSeconds(licenseSnapshot.EndDate.Seconds)
                : NodaTime.Instant.MaxValue;
            license.Modules = licenseSnapshot.Modules.Select(m => new Models.LicenseModule
            {
                Id = Guid.TryParse(m.Id, out var mid) ? mid : Guid.Empty,
                Name = m.Name,
                Description = m.Description
            }).ToList();
            license.Metadata = licenseSnapshot.Metadata?.ToDictionary(k => k.Key, v => v.Value) ?? [];
        }
        else if (proto is not null)
        {
            license.Id = Guid.TryParse(proto.Id, out var pid) ? pid : Guid.Empty;
            license.Name = proto.Name;
            license.StartDate = NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Parse(proto.StartDate));
            license.ExpirationDate = NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Parse(proto.EndDate));
            license.Metadata = proto.Metadata?.ToDictionary(k => k.Key, v => v.Value) ?? [];
        }

        return license;
    }

    private static VoLocation.Location BuildLocation(TenantProto.Location? proto)
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
