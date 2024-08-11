using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.File.Storage.Test.Providers;

public class ProviderTest
{
    [Fact]
    public async Task ProcessAsync_NotEnable_Null()
    {
        // Arrange
        var logger = new Mock<ILogger<FakeProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";

        var provider = new FakeProvider(logger.Object, environment.Object);

        // Act
        var result = await provider.InvokeProcessAsync(false, filename, TypeProviders.AzureBlobProvider, (f, r) =>
        {
            return Task.FromResult(r);
        });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ProcessAsync_Enable_NotNull()
    {
        // Arrange
        var logger = new Mock<ILogger<FakeProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";

        var provider = new FakeProvider(logger.Object, environment.Object);

        // Act
        var result = await provider.InvokeProcessAsync(true, filename, TypeProviders.AzureBlobProvider, (f, r) =>
        {
            Assert.NotNull(f);
            Assert.NotNull(r);
            Assert.Equal(filename, f.Name);
            Assert.Equal(TypeProviders.AzureBlobProvider.ToString(), r.Provider);
            Assert.Equal(f, r.File);

            return Task.FromResult(r);
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TypeProviders.AzureBlobProvider.ToString(), result.Provider);
        Assert.NotNull(result.File);
    }

    [Fact]
    public async Task ProcessAsync_InternalError_Failed()
    {
        // Arrange
        var logger = new Mock<ILogger<FakeProvider>>();
        var environment = new Mock<IHostEnvironment>();
        environment.Setup(x => x.EnvironmentName).Returns("Development");

        var filename = "test.txt";

        var provider = new FakeProvider(logger.Object, environment.Object);

        // Act
        var result = await provider.InvokeProcessAsync(true, filename, TypeProviders.AzureBlobProvider, (f, r) =>
        {
            throw new Exception("Internal error");
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Internal error", result.Message);
        Assert.NotNull(result.File);
        Assert.NotNull(result.Exception);
    }
}
