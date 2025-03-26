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
}
