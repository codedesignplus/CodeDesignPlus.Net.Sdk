using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Services;
using Microsoft.Extensions.Logging;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Services;

public class FileStorageServiceTest
{
    [Fact]
    public async Task UploadAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<FileStorageService>>();
        var options = new Mock<IOptions<FileStorageOptions>>();
        var filename = "file.txt";
        var file = new M.File(filename);
        var responseBlob = new M.Response(file, TypeProviders.AzureBlobProvider) { Success = true };
        var responseFile = new M.Response(file, TypeProviders.AzureFileProvider) { Success = true };
        var responseLocal = new M.Response(file, TypeProviders.LocalProvider) { Success = true };

        var azureBlobProviderMock = new Mock<IAzureBlobProvider>();
        var azureFileProviderMock = new Mock<IAzureFileProvider>();
        var localProviderMock = new Mock<ILocalProvider>();

        azureBlobProviderMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseBlob);

        azureFileProviderMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFile);

        localProviderMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseLocal);

        var providers = new List<IProvider>()
        {
            azureBlobProviderMock.Object,
            azureFileProviderMock.Object,
            localProviderMock.Object
        };

        var service = new FileStorageService(logger.Object, options.Object, providers);

        // Act
        var result = await service.UploadAsync(new MemoryStream(), "file.txt", "target", true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);
    }
}
