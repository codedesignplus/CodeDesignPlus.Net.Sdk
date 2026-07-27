using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using M = CodeDesignPlus.Net.Security.Abstractions.Models;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class TenantTest
{
    private readonly Mock<ILogger<Tenant>> loggerMock;
    private readonly Mock<ITenantDirectory> directoryMock;
    private readonly Tenant tenantService;

    public TenantTest()
    {
        loggerMock = new Mock<ILogger<Tenant>>();
        directoryMock = new Mock<ITenantDirectory>();
        tenantService = new Tenant(loggerMock.Object, directoryMock.Object);
    }

    private static Currency CreateCurrency(string code = "USD", short numericCode = 840) =>
        Currency.Create(Guid.NewGuid(), "US Dollar", code, "$", 2, numericCode);

    private static Country CreateCountry(string timezone = "America/New_York", Currency? currency = null) =>
        Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "+1", timezone, currency ?? CreateCurrency());

    private static State CreateState() =>
        State.Create(Guid.NewGuid(), "New York", "NY");

    private static City CreateCity(string? timezone = null) =>
        City.Create(Guid.NewGuid(), "New York", timezone);

    private static Locality CreateLocality() =>
        Locality.Create(Guid.NewGuid(), "Manhattan");

    private static Neighborhood CreateNeighborhood() =>
        Neighborhood.Create(Guid.NewGuid(), "Midtown");

    private static Location CreateLocation(Country? country = null, State? state = null, City? city = null, Locality? locality = null, Neighborhood? neighborhood = null) =>
        Location.Create(
            country ?? CreateCountry(),
            state ?? CreateState(),
            city ?? CreateCity(),
            locality ?? CreateLocality(),
            neighborhood ?? CreateNeighborhood(),
            "123 Main St",
            "10001"
        );

    [Fact]
    public async Task SetAsync_TenantExists_SetsTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new M.Tenant { Id = tenantId };
        directoryMock.Setup(d => d.GetSnapshotAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);

        // Act
        await tenantService.SetAsync(tenantId);

        // Assert
        Assert.True(tenantService.IsLoaded);
        directoryMock.Verify(d => d.GetSnapshotAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
        loggerMock.VerifyLogging($"Tenant loaded: {tenantId}", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task SetAsync_TenantCannotBeResolved_ThrowsSecurityException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        directoryMock.Setup(d => d.GetSnapshotAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync((M.Tenant)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SecurityException>(() => tenantService.SetAsync(tenantId));

        Assert.Equal($"The tenant {tenantId} could not be resolved.", exception.Message);
        Assert.False(tenantService.IsLoaded);
    }

    [Fact]
    public async Task SetAsync_CalledRepeatedly_ReplacesTheLoadedTenant()
    {
        // Arrange: es el caso de un job recurrente que itera tenants dentro del mismo scope.
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        directoryMock.Setup(d => d.GetSnapshotAsync(first, It.IsAny<CancellationToken>())).ReturnsAsync(new M.Tenant { Id = first });
        directoryMock.Setup(d => d.GetSnapshotAsync(second, It.IsAny<CancellationToken>())).ReturnsAsync(new M.Tenant { Id = second });

        // Act
        await tenantService.SetAsync(first);
        await tenantService.SetAsync(second);

        // Assert
        Assert.True(tenantService.IsLoaded);
        directoryMock.Verify(d => d.GetSnapshotAsync(first, It.IsAny<CancellationToken>()), Times.Once);
        directoryMock.Verify(d => d.GetSnapshotAsync(second, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Members_WithoutLoadedTenant_ThrowSecurityException()
    {
        // Act & Assert
        Assert.False(tenantService.IsLoaded);
        Assert.Throws<SecurityException>(() => tenantService.Country);
        Assert.Throws<SecurityException>(() => tenantService.Currency);
        Assert.Throws<SecurityException>(() => tenantService.Metadata);
        Assert.Throws<SecurityException>(() => tenantService.Modules);
        Assert.Throws<SecurityException>(() => tenantService.LicenseIsValid());
    }

    [Fact]
    public void LicenseIsValid_ValidLicense_ReturnsTrue()
    {
        // Arrange
        var tenant = new M.Tenant
        {
            License = new M.License
            {
                StartDate = NodaTime.SystemClock.Instance.GetCurrentInstant().Minus(NodaTime.Duration.FromDays(1)),
                ExpirationDate = NodaTime.SystemClock.Instance.GetCurrentInstant().Plus(NodaTime.Duration.FromDays(1))
            }
        };

        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act
        var result = tenantService.LicenseIsValid();

        // Assert
        Assert.True(result);
        loggerMock.VerifyLogging($"The license with id {tenant.License.Id} is valid: True, StartDate: {tenant.License.StartDate}, ExpirationDate: {tenant.License.ExpirationDate}", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public void LicenseIsValid_InvalidLicense_ReturnsFalse()
    {
        // Arrange
        var tenant = new M.Tenant
        {
            License = new M.License
            {
                StartDate = NodaTime.SystemClock.Instance.GetCurrentInstant().Plus(NodaTime.Duration.FromDays(1)),
                ExpirationDate = NodaTime.SystemClock.Instance.GetCurrentInstant().Plus(NodaTime.Duration.FromDays(2))
            }
        };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act
        var result = tenantService.LicenseIsValid();

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging($"The license with id {tenant.License.Id} is valid: False, StartDate: {tenant.License.StartDate}, ExpirationDate: {tenant.License.ExpirationDate}", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public void GetMetadata_ValidKey_ReturnsValue()
    {
        // Arrange
        var key = "testKey";
        var value = "testValue";
        var tenant = new M.Tenant
        {
            Metadata = new Dictionary<string, string> { { key, value } }
        };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act
        var result = tenantService.GetMetadata(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void GetMetadata_InvalidKey_ThrowsKeyNotFoundException()
    {
        // Arrange
        var key = "invalidKey";
        var tenant = new M.Tenant { Metadata = [] };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => tenantService.GetMetadata(key));
    }

    [Fact]
    public void GetMetadata_Generic_ValidKey_ReturnsValue()
    {
        // Arrange
        var key = "testKey";
        var value = "123";
        var tenant = new M.Tenant
        {
            Metadata = new Dictionary<string, string> { { key, value } }
        };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act
        var result = tenantService.GetMetadata<int>(key);

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void GetMetadata_Generic_InvalidKey_ThrowsKeyNotFoundException()
    {
        // Arrange
        var key = "invalidKey";
        var tenant = new M.Tenant { Metadata = [] };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => tenantService.GetMetadata<int>(key));

        Assert.Equal($"The given key '{key}' was not present in the dictionary.", exception.Message);
    }

    [Fact]
    public void Country_ReturnsCountry()
    {
        // Arrange
        var country = CreateCountry();
        var tenant = new M.Tenant { Location = CreateLocation(country: country) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.Country;

        // Assert
        Assert.Equal(country, result);
    }

    [Fact]
    public void State_ReturnsState()
    {
        // Arrange
        var state = CreateState();
        var tenant = new M.Tenant { Location = CreateLocation(state: state) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.State;

        // Assert
        Assert.Equal(state, result);
    }

    [Fact]
    public void City_ReturnsCity()
    {
        // Arrange
        var city = CreateCity();
        var tenant = new M.Tenant { Location = CreateLocation(city: city) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.City;

        // Assert
        Assert.Equal(city, result);
    }

    [Fact]
    public void Locality_ReturnsLocality()
    {
        // Arrange
        var locality = CreateLocality();
        var tenant = new M.Tenant { Location = CreateLocation(locality: locality) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.Locality;

        // Assert
        Assert.Equal(locality, result);
    }

    [Fact]
    public void Neighborhood_ReturnsNeighborhood()
    {
        // Arrange
        var neighborhood = CreateNeighborhood();
        var tenant = new M.Tenant { Location = CreateLocation(neighborhood: neighborhood) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.Neighborhood;

        // Assert
        Assert.Equal(neighborhood, result);
    }

    [Fact]
    public void TimeZone_ReturnsCityTimeZone_WhenCityTimeZoneIsNotNull()
    {
        // Arrange
        var city = City.Create(Guid.NewGuid(), "New York", "CityTZ");
        var currency = Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);
        var country = Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "+1", "CountryTZ", currency);
        var tenant = new M.Tenant { Location = CreateLocation(country: country, city: city) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.TimeZone;

        // Assert
        Assert.Equal("CityTZ", result);
    }

    [Fact]
    public void TimeZone_ReturnsCountryTimeZone_WhenCityTimeZoneIsNull()
    {
        // Arrange
        var city = City.Create(Guid.NewGuid(), "New York", null);
        var currency = Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);
        var country = Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "+1", "CountryTZ", currency);
        var tenant = new M.Tenant { Location = CreateLocation(country: country, city: city) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.TimeZone;

        // Assert
        Assert.Equal("CountryTZ", result);
    }

    [Fact]
    public void Currency_ReturnsCountryCurrency()
    {
        // Arrange
        var currency = Currency.Create(Guid.NewGuid(), "Colombian Peso", "COP", "$", 2, 170);
        var country = Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", "America/Bogota", currency);
        var tenant = new M.Tenant { Location = CreateLocation(country: country) };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.Currency;

        // Assert
        Assert.Equal(currency, result);
    }

    [Fact]
    public void Metadata_ReturnsTenantMetadata()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { { "key", "value" } };
        var tenant = new M.Tenant { Metadata = metadata };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.Metadata;

        // Assert
        Assert.Equal(metadata, result);
    }

    [Fact]
    public void GetMetadata_Generic_ConvertsToCorrectType()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { { "testKey", "123" } };
        var tenant = new M.Tenant { Metadata = metadata };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act
        var result = tenantService.GetMetadata<int>("testKey");

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void GetMetadata_KeyNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var key = "nonExistentKey";
        var tenant = new M.Tenant { Metadata = [] };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => tenantService.GetMetadata(key));
    }

    [Fact]
    public void GetMetadata_Generic_InvalidCast_ThrowsInvalidCastException()
    {
        // Arrange
        var key = "invalidInt";
        var value = "notAnInt";
        var tenant = new M.Tenant { Metadata = new Dictionary<string, string> { { key, value } } };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(tenantService, tenant);

        // Act & Assert
        Assert.Throws<FormatException>(() => tenantService.GetMetadata<int>(key));
    }
}
