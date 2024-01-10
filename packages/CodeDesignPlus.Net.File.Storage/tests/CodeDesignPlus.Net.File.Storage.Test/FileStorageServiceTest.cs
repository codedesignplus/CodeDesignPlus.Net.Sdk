using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Services;

public class FileStorageServiceTest
{
    [Fact]
    public async Task UploadAsync_AllProviders_Success()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var logger = new Mock<ILogger<FileStorageService<Guid, Guid>>>();
        var options = new Mock<IOptions<FileStorageOptions>>();
        var filename = "file.txt";
        var target = "target";
        var renowned = true;
        var file = new M.File(filename);
        var stream = new MemoryStream();

        var responseBlob = new M.Response(file, TypeProviders.AzureBlobProvider) { Success = true };
        var responseFile = new M.Response(file, TypeProviders.AzureFileProvider) { Success = true };
        var responseLocal = new M.Response(file, TypeProviders.LocalProvider) { Success = true };

        var azureBlobProviderMock = new Mock<IAzureBlobProvider<Guid, Guid>>();
        var azureFileProviderMock = new Mock<IAzureFileProvider<Guid, Guid>>();
        var localProviderMock = new Mock<ILocalProvider<Guid, Guid>>();

        azureBlobProviderMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, string, string, bool, CancellationToken>((s, f, t, r, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(renowned, r);
                Assert.Equal(stream, s);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseBlob)
            .Verifiable();

        azureFileProviderMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, string, string, bool, CancellationToken>((s, f, t, r, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(renowned, r);
                Assert.Equal(stream, s);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseFile)
            .Verifiable();

        localProviderMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, string, string, bool, CancellationToken>((s, f, t, r, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(renowned, r);
                Assert.Equal(stream, s);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseLocal)
            .Verifiable();

        var providers = new List<IProvider<Guid, Guid>>()
        {
            azureBlobProviderMock.Object,
            azureFileProviderMock.Object,
            localProviderMock.Object
        };

        var service = new FileStorageService<Guid, Guid>(logger.Object, options.Object, providers);

        // Act
        var result = await service.UploadAsync(stream, filename, target, renowned, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(responseBlob, result);
        Assert.Contains(responseFile, result);
        Assert.Contains(responseLocal, result);

        azureBlobProviderMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        azureFileProviderMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        localProviderMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_InvokeFirstProvider_Success()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var logger = new Mock<ILogger<FileStorageService<Guid, Guid>>>();
        var options = new Mock<IOptions<FileStorageOptions>>();
        var filename = "file.txt";
        var target = "target";
        var file = new M.File(filename);

        var responseBlob = new M.Response(file, TypeProviders.AzureBlobProvider) { Success = true };
        var responseFile = new M.Response(file, TypeProviders.AzureFileProvider) { Success = true };
        var responseLocal = new M.Response(file, TypeProviders.LocalProvider) { Success = true };

        var azureBlobProviderMock = new Mock<IAzureBlobProvider<Guid, Guid>>();
        var azureFileProviderMock = new Mock<IAzureFileProvider<Guid, Guid>>();
        var localProviderMock = new Mock<ILocalProvider<Guid, Guid>>();

        azureBlobProviderMock
            .Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((f, t, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseBlob)
            .Verifiable();

        var providers = new List<IProvider<Guid, Guid>>()
        {
            azureBlobProviderMock.Object,
            azureFileProviderMock.Object,
            localProviderMock.Object
        };

        var service = new FileStorageService<Guid, Guid>(logger.Object, options.Object, providers);

        // Act
        var result = await service.DownloadAsync(filename, target, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(responseBlob, result);
        azureBlobProviderMock.Verify(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_InvokeSecondProvider_Success()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var logger = new Mock<ILogger<FileStorageService<Guid, Guid>>>();
        var options = new Mock<IOptions<FileStorageOptions>>();
        var filename = "file.txt";
        var target = "target";
        var file = new M.File(filename);

        var responseBlob = new M.Response(file, TypeProviders.AzureBlobProvider) { Success = false };
        var responseFile = new M.Response(file, TypeProviders.AzureFileProvider) { Success = true };
        var responseLocal = new M.Response(file, TypeProviders.LocalProvider) { Success = true };

        var azureBlobProviderMock = new Mock<IAzureBlobProvider<Guid, Guid>>();
        var azureFileProviderMock = new Mock<IAzureFileProvider<Guid, Guid>>();
        var localProviderMock = new Mock<ILocalProvider<Guid, Guid>>();

        azureBlobProviderMock
            .Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((f, t, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseBlob)
            .Verifiable();

        azureFileProviderMock
            .Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((f, t, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseFile)
            .Verifiable();

        var providers = new List<IProvider<Guid, Guid>>()
        {
            azureBlobProviderMock.Object,
            azureFileProviderMock.Object,
            localProviderMock.Object
        };

        var service = new FileStorageService<Guid, Guid>(logger.Object, options.Object, providers);

        // Act
        var result = await service.DownloadAsync(filename, target, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(responseFile, result);
        azureBlobProviderMock.Verify(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        azureFileProviderMock.Verify(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_EmptyProviders_ReturnNull()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var logger = new Mock<ILogger<FileStorageService<Guid, Guid>>>();
        var options = new Mock<IOptions<FileStorageOptions>>();
        var filename = "file.txt";
        var target = "target";

        var providers = new List<IProvider<Guid, Guid>>();

        var service = new FileStorageService<Guid, Guid>(logger.Object, options.Object, providers);

        // Act
        var result = await service.DownloadAsync(filename, target, cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_InvokeProviders_Success()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var logger = new Mock<ILogger<FileStorageService<Guid, Guid>>>();
        var options = new Mock<IOptions<FileStorageOptions>>();
        var filename = "file.txt";
        var target = "target";

        var responseBlob = new M.Response(new M.File(filename), TypeProviders.AzureBlobProvider) { Success = true };
        var responseFile = new M.Response(new M.File(filename), TypeProviders.AzureFileProvider) { Success = true };
        var responseLocal = new M.Response(new M.File(filename), TypeProviders.LocalProvider) { Success = true };

        var azureBlobProviderMock = new Mock<IAzureBlobProvider<Guid, Guid>>();
        var azureFileProviderMock = new Mock<IAzureFileProvider<Guid, Guid>>();
        var localProviderMock = new Mock<ILocalProvider<Guid, Guid>>();

        azureBlobProviderMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((f, t, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseBlob)
            .Verifiable();

        azureFileProviderMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((f, t, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseFile)
            .Verifiable();

        localProviderMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((f, t, c) =>
            {
                Assert.Equal(filename, f);
                Assert.Equal(target, t);
                Assert.Equal(cancellationToken, c);
            })
            .ReturnsAsync(responseLocal)
            .Verifiable();

        var providers = new List<IProvider<Guid, Guid>>()
        {
            azureBlobProviderMock.Object,
            azureFileProviderMock.Object,
            localProviderMock.Object
        };

        var service = new FileStorageService<Guid, Guid>(logger.Object, options.Object, providers);

        // Act
        var result = await service.DeleteAsync(filename, target, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(responseBlob, result);
        Assert.Contains(responseFile, result);
        Assert.Contains(responseLocal, result);
        azureBlobProviderMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        azureFileProviderMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        localProviderMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

    }
}
