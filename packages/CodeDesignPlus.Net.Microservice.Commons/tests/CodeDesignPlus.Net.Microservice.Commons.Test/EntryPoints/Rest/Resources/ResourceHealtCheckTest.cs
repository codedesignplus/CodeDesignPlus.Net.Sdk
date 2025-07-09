using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Resources;

public class ResourceHealtCheckTest
{
    [Fact]
    public async Task CheckHealthAsync_WhenRegisterResourcesCompletedIsTrue_ReturnsHealthy()
    {
        // Arrange
        var healthCheck = new ResourceHealtCheck
        {
            RegisterResourcesCompleted = true
        };
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("The Resource is ready.", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenRegisterResourcesCompletedIsFalse_ReturnsUnhealthy()
    {
        // Arrange
        var healthCheck = new ResourceHealtCheck
        {
            RegisterResourcesCompleted = false
        };
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("That Resource is not ready.", result.Description);
    }
}
