using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Test.Helpers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.File.Storage.Test.Providers;

public class LocalProviderTest
{
    [Fact]
    public async Task UploadAsync_Enable_Success()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var fileOptions = OptionsUtil.FileStorageOptions;
        var options = Microsoft.Extensions.Options.Options.Create(fileOptions);
        var logger = new Mock<ILogger<LocalProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var target = "test";

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.Tenant).Returns(tenant);

        var provider = new LocalProvider(options, logger.Object, environment.Object, userContext.Object);

        // Act
        var response = await provider.UploadAsync(stream, filename, target);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.File);
        Assert.NotNull(response.File.Detail);
        Assert.Equal(TypeProviders.LocalProvider.ToString(), response.File.Detail.Provider);
        Assert.Equal(target, response.File.Detail.Target);
        Assert.Equal(filename, response.File.Detail.File);
        Assert.NotNull(response.File.Detail.Uri);
        Assert.NotNull(response.File.Detail.UriDownload);
        Assert.NotNull(response.File.Detail.UriViewInBrowser);
    }


    [Fact]
    public async Task UploadAsync_Renowed_Success()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var fileOptions = OptionsUtil.FileStorageOptions;
        var options = Microsoft.Extensions.Options.Options.Create(fileOptions);
        var logger = new Mock<ILogger<LocalProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var target = "test";
        var renowned = true;

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.Tenant).Returns(tenant);

        var provider = new LocalProvider(options, logger.Object, environment.Object, userContext.Object);

        // Act
        var result = await provider.UploadAsync(stream, filename, target, renowned);
        var response = await provider.UploadAsync(stream, filename, target, renowned);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.File);
        Assert.NotNull(response.File.Detail);
        Assert.Equal(TypeProviders.LocalProvider.ToString(), response.File.Detail.Provider);
        Assert.Equal(target, response.File.Detail.Target);
        Assert.Equal("test (2).txt", response.File.Detail.File);
        Assert.NotNull(response.File.Detail.Uri);
        Assert.NotNull(response.File.Detail.UriDownload);
        Assert.NotNull(response.File.Detail.UriViewInBrowser);
    }

    [Fact]
    public async Task DeleteAsync_FileExist_Success()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var fileOptions = OptionsUtil.FileStorageOptions;
        var options = Microsoft.Extensions.Options.Options.Create(fileOptions);
        var logger = new Mock<ILogger<LocalProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var target = "test";

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.Tenant).Returns(tenant);

        var provider = new LocalProvider(options, logger.Object, environment.Object, userContext.Object);

        // Act
        var response = await provider.UploadAsync(stream, filename, target);

        // Assert
        Assert.True(response.Success);

        // Act
        var responseDelete = await provider.DeleteAsync(filename, target);

        // Assert
        Assert.True(responseDelete.Success);
    }

    [Fact]
    public async Task DeleteAsync_FileNotExist_Failed()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var fileOptions = OptionsUtil.FileStorageOptions;
        var options = Microsoft.Extensions.Options.Options.Create(fileOptions);
        var logger = new Mock<ILogger<LocalProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var target = "test";

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.Tenant).Returns(tenant);

        var provider = new LocalProvider(options, logger.Object, environment.Object, userContext.Object);

        // Act
        var responseDelete = await provider.DeleteAsync(filename, target);

        // Assert
        Assert.False(responseDelete.Success);
        Assert.Equal("The system cannot find the file specified", responseDelete.Message);
    }

    [Fact]
    public async Task DownloadAsync_FileExist_Success()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var fileOptions = OptionsUtil.FileStorageOptions;
        var options = Microsoft.Extensions.Options.Options.Create(fileOptions);
        var logger = new Mock<ILogger<LocalProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var target = "test";

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.Tenant).Returns(tenant);

        var provider = new LocalProvider(options, logger.Object, environment.Object, userContext.Object);

        // Act
        var response = await provider.UploadAsync(stream, filename, target);

        // Assert
        Assert.True(response.Success);

        // Act
        var responseDownload = await provider.DownloadAsync(filename, target);

        // Assert
        Assert.True(responseDownload.Success);
        Assert.NotNull(responseDownload.Stream);
        stream.Position = 0;
        Assert.True(Helpers.Extensions.CompareStreams(stream, responseDownload.Stream));
    }

    [Fact]
    public async Task DownloadAsync_FileNotExist_Success()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var fileOptions = OptionsUtil.FileStorageOptions;
        var options = Microsoft.Extensions.Options.Options.Create(fileOptions);
        var logger = new Mock<ILogger<LocalProvider>>();
        var environment = new Mock<IHostEnvironment>();
        var filename = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var target = "test";

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.Tenant).Returns(tenant);

        var provider = new LocalProvider(options, logger.Object, environment.Object, userContext.Object);

        // Act
        var responseDownload = await provider.DownloadAsync(filename, target);

        // Assert
        Assert.False(responseDownload.Success);
        Assert.Equal("The system cannot find the file specified", responseDownload.Message);
    }
}
