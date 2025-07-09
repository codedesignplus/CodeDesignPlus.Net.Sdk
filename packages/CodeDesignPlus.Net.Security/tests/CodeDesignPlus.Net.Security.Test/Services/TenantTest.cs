using System;
using System.Collections.Generic;
using System.Security;
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
    private readonly Mock<ICacheManager> cacheManagerMock;
    private readonly Tenant tenantService;

    public TenantTest()
    {
        loggerMock = new Mock<ILogger<Tenant>>();
        cacheManagerMock = new Mock<ICacheManager>();
        tenantService = new Tenant(loggerMock.Object, cacheManagerMock.Object);
    }

    [Fact]
    public async Task SetTenantAsync_TenantExists_SetsTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new M.Tenant { Id = tenantId };
        cacheManagerMock.Setup(cm => cm.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        cacheManagerMock.Setup(cm => cm.GetAsync<M.Tenant>(It.IsAny<string>())).ReturnsAsync(tenant);

        // Act
        await tenantService.SetTenantAsync(tenantId);

        // Assert
        cacheManagerMock.Verify(cm => cm.ExistsAsync($"Tenant:{tenantId}"), Times.Once);
        cacheManagerMock.Verify(cm => cm.GetAsync<M.Tenant>($"Tenant:{tenantId}"), Times.Once);
        loggerMock.VerifyLogging($"Tenant loaded: {tenantId}", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task SetTenantAsync_TenantDoesNotExist_ThrowsSecurityException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        cacheManagerMock.Setup(cm => cm.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Security.Exceptions.SecurityException>(() => tenantService.SetTenantAsync(tenantId));

        Assert.Equal("The tenant specified does not exist at the level of the cache.", exception.Message);
        cacheManagerMock.Verify(cm => cm.ExistsAsync($"Tenant:{tenantId}"), Times.Once);
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
        var tenant = new M.Tenant
        {
            Metadata = []
        };
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
        var tenant = new M.Tenant
        {
            Metadata = []
        };
        tenantService.GetType().GetField("tenant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(tenantService, tenant);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => tenantService.GetMetadata<int>(key));

        Assert.Equal($"The given key '{key}' was not present in the dictionary.", exception.Message);
    }

    [Fact]
    public void Country_ReturnsCountry()
    {
        // Arrange
        var country = new M.Country { Name = "TestCountry" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                Country = country
            }
        };
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
        var state = new M.State { Name = "TestState" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                State = state
            }
        };
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
        var city = new M.City { Name = "TestCity" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                City = city
            }
        };
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
        var locality = new M.Locality { Name = "TestLocality" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                Locality = locality
            }
        };
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
        var neighborhood = new M.Neighborhood { Name = "TestNeighborhood" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                Neighborhood = neighborhood
            }
        };
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
        var city = new M.City { TimeZone = "CityTZ" };
        var country = new M.Country { TimeZone = "CountryTZ" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                City = city,
                Country = country
            }
        };
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
        var city = new M.City { TimeZone = null };
        var country = new M.Country { TimeZone = "CountryTZ" };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                City = city,
                Country = country
            }
        };
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
        var currency = new M.Currency { Code = "USD" };
        var country = new M.Country { Currency = currency };
        var tenant = new M.Tenant
        {
            Location = new M.Location
            {
                Country = country
            }
        };
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
        var tenant = new M.Tenant
        {
            Metadata = metadata
        };
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
        var metadata = new Dictionary<string, string> {
            { "testKey", "123" }
        };
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
