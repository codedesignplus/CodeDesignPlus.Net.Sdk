using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Providers;

public class AzureFileProviderTest
{
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly CancellationToken cancellationToken;
    private readonly Guid tenant;
    private readonly string filename;
    private readonly string path;
    private readonly FileStream stream;
    private string target;
    private M.FileDetail pathDetail;
    private readonly M.File file;
    private readonly Mock<ILogger<AzureFileProvider>> loggerMock;
    private readonly Mock<IHostEnvironment> environmentMock;
    private readonly IOptions<FileStorageOptions> options;
    private readonly Mock<ShareServiceClient> shareServiceClientMock;
    private readonly Mock<ShareClient> shareClientMock;
    private readonly Mock<ShareDirectoryClient> shareDirectoryClientMock;
    private readonly Mock<ShareFileClient> shareFileClientMock;
    private readonly Mock<IAzureFileFactory> factoryMock;
    private readonly Mock<IUserContext> userContextMock;

    public AzureFileProviderTest()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = cancellationTokenSource.Token;
        this.tenant = Guid.NewGuid();
        this.filename = "FakeDocument.txt";
        this.path = Path.Combine(AppContext.BaseDirectory, "Helpers", "Files", filename);
        this.stream = System.IO.File.OpenRead(path);
        this.target = "docs/general";
        this.pathDetail = new M.FileDetail(OptionsUtil.FileStorageOptions.UriDownload, target, this.filename, TypeProviders.AzureFileProvider);
        this.file = new M.File(filename);
        this.options = O.Options.Create(OptionsUtil.FileStorageOptions);

        this.loggerMock = new Mock<ILogger<AzureFileProvider>>();
        this.userContextMock = new Mock<IUserContext>();
        this.environmentMock = new Mock<IHostEnvironment>();
        this.shareServiceClientMock = new Mock<ShareServiceClient>();
        this.shareClientMock = new Mock<ShareClient>();
        this.shareDirectoryClientMock = new Mock<ShareDirectoryClient>();
        this.shareFileClientMock = new Mock<ShareFileClient>();
        this.factoryMock = new Mock<IAzureFileFactory>();

        userContextMock.SetupGet(x => x.Tenant).Returns(tenant);

        factoryMock.SetupGet(x => x.Options).Returns(options.Value);
        factoryMock.SetupGet(x => x.UserContext).Returns(userContextMock.Object);

        factoryMock.Setup(x => x.GetContainerClient()).Returns(shareClientMock.Object).Verifiable();
        factoryMock.Setup(x => x.Create()).Returns(factoryMock.Object).Verifiable();

        shareClientMock.Setup(x => x.GetDirectoryClient(It.IsAny<string>())).Returns(shareDirectoryClientMock.Object).Verifiable();

        shareFileClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<ShareFileUploadOptions>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, ShareFileUploadOptions, CancellationToken>((s, options, t) =>
            {
                Assert.Equal(stream, s);
                Assert.Equal(cancellationToken, t);
            });

        shareFileClientMock
            .Setup(x => x.DownloadAsync(It.IsAny<ShareFileDownloadOptions>(), It.IsAny<CancellationToken>()))
            .Callback<ShareFileDownloadOptions, CancellationToken>((o, t) =>
            {
                Assert.Equal(cancellationToken, t);
            })
            .ReturnsAsync(Azure.Response.FromValue(FilesModelFactory.StorageFileDownloadInfo(content: stream), Mock.Of<Azure.Response>()));

        shareFileClientMock
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>()))
            .Callback<ShareFileRequestConditions, CancellationToken>((c, t) =>
            {
                Assert.Equal(cancellationToken, t);
            })
            .ReturnsAsync(Azure.Response.FromValue(true, Mock.Of<Azure.Response>()))
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(true, Mock.Of<Azure.Response>()))
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<ShareFileHttpHeaders>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<FileSmbProperties>(), It.IsAny<string>(), It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>()))
            .Callback<long, ShareFileHttpHeaders, IDictionary<string, string>, FileSmbProperties, string, ShareFileRequestConditions, CancellationToken>((l, h, m, s, x, c, t) =>
            {
                Assert.Equal(stream.Length, l);
                Assert.Contains(file.GetMetadata(pathDetail.Uri), x => m.Any(o => o.Key == x.Key));
                Assert.Equal(cancellationToken, t);
            })
            .ReturnsAsync(Azure.Response.FromValue((ShareFileInfo)null!, Mock.Of<Azure.Response>()))
            .Verifiable();
    }

    [Fact]
    public async Task UploadAsync_Default_Success()
    {
        // Arrange
        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, cancellationToken: cancellationToken);

        // Assert
        AssertUpload(result);
    }

    [Fact]
    public async Task UploadAsync_EmptyTarget_Success()
    {
        // Arrange
        this.target = null!;
        this.pathDetail = new M.FileDetail(OptionsUtil.FileStorageOptions.UriDownload, target, filename, TypeProviders.AzureFileProvider);

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, cancellationToken: cancellationToken);

        // Assert
        AssertUpload(result);
    }

    [Fact]
    public async Task UploadAsync_Renowned_Success()
    {
        // Arrange
        file.Renowned = true;
        var name = $"{target}/{file.Name} ({2}){file.Extension}";
        this.pathDetail = new M.FileDetail(OptionsUtil.FileStorageOptions.UriDownload, target, System.IO.Path.GetFileName(name), TypeProviders.AzureFileProvider);

        var existContainer = true;
        shareFileClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .Callback<CancellationToken>(t =>
            {
                Assert.Equal(cancellationToken, t);
            })
            .ReturnsAsync(() =>
            {
                var result = Azure.Response.FromValue(existContainer, Mock.Of<Azure.Response>());

                existContainer = false;

                return result;
            })
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, true, cancellationToken);

        // Assert
        AssertUpload(result, 2);
    }


    [Fact]
    public async Task UploadAsync_DirectoryNotExist_Success()
    {
        // Arrange        
        shareDirectoryClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()))
            .Verifiable();

        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t =>
            {
                Assert.Contains(t, $"{target}/");
            })
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, cancellationToken: cancellationToken);

        // Assert
        AssertUpload(result);
    }

    private void AssertUpload(M.Response result, int version = 1)
    {
        Assert.True(result.Success);

        shareClientMock.Verify(x => x.CreateIfNotExistsAsync(It.IsAny<ShareCreateOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        shareDirectoryClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<ShareFileCreateOptions>(), It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
        shareFileClientMock.Verify(x => x.UploadAsync(stream, It.IsAny<ShareFileUploadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);

        Assert.True(result.Success);
        Assert.Equal(stream.Length, result.File.Size);
        Assert.Equal(file.Renowned, result.File.Renowned);
        Assert.Equal(version, result.File.Version.Major);

        Assert.Equal(pathDetail.Target, result.File.Detail.Target);
        Assert.Equal(pathDetail.File, result.File.Detail.File);
        Assert.Equal(pathDetail.Provider, result.File.Detail.Provider);
        Assert.Equal(pathDetail.UriDownload, result.File.Detail.UriDownload);
        Assert.Equal(pathDetail.UriViewInBrowser, result.File.Detail.UriViewInBrowser);
        Assert.Equal(pathDetail.Uri, result.File.Detail.Uri);
    }


    [Fact]
    public async Task DownloadAsync_FileExist_Success()
    {
        // Arrange
        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(target, t))
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(filename, t))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(true, Mock.Of<Azure.Response>()))
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.DownloadAsync(this.filename, this.target, cancellationToken);

        // Assert
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);

        this.stream.Position = 0;
        Assert.True(result.Success);
        Assert.Equal(stream.Length, result.Stream!.Length);
        Assert.Equal(0, result.Stream.Position);
        Assert.Equal(stream, result.Stream);
    }


    [Fact]
    public async Task DownloadAsync_FileNotExist_Success()
    {
        // Arrange
        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(target, t))
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(filename, t))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()))
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.DownloadAsync(this.filename, this.target, cancellationToken);

        // Assert
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.Success);
        Assert.Equal($"The file {filename} does not exist in the container {tenant}", result.Message);
    }

    [Fact]
    public async Task GetSignedUrlAsync_FileExists_ReturnsSuccess()
    {
        // Arrange
        var sasUri = new Uri("https://fakeaccount.file.core.windows.net/share/docs/general/FakeDocument.txt?sastoken");
        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(target, t))
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(filename, t))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(true, Mock.Of<Azure.Response>()))
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.GenerateSasUri(It.IsAny<Azure.Storage.Sas.ShareFileSasPermissions>(), It.IsAny<DateTimeOffset>()))
            .Returns(sasUri)
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);
        var timeSpan = TimeSpan.FromMinutes(30);

        // Act
        var result = await provider.GetSignedUrlAsync(filename, target, timeSpan, cancellationToken);

        // Assert
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        shareFileClientMock.Verify(x => x.GenerateSasUri(It.IsAny<Azure.Storage.Sas.ShareFileSasPermissions>(), It.IsAny<DateTimeOffset>()), Times.Once);

        Assert.True(result.Success);
        Assert.NotNull(result.File.Detail);
        Assert.Equal(sasUri, result.File.Detail.SignedUrl);
        Assert.Equal(target, result.File.Detail.Target);
        Assert.Equal(filename, result.File.Detail.File);
    }

    [Fact]
    public async Task GetSignedUrlAsync_FileDoesNotExist_ReturnsFailure()
    {
        // Arrange
        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(target, t))
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(filename, t))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()))
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);
        var timeSpan = TimeSpan.FromMinutes(30);

        // Act
        var result = await provider.GetSignedUrlAsync(filename, target, timeSpan, cancellationToken);

        // Assert
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.Success);
        Assert.Equal($"The file {filename} does not exist in the container {tenant}", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_FileExist_Success()
    {
        // Arrange
        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(target, t))
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(filename, t))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.DeleteAsync(this.filename, this.target, cancellationToken);

        // Assert
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.DeleteIfExistsAsync(It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.True(result.Success);
    }


    [Fact]
    public async Task DeleteAsync_FileNotExist_Success()
    {
        // Arrange
        shareClientMock
            .Setup(x => x.GetDirectoryClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(target, t))
            .Returns(shareDirectoryClientMock.Object)
            .Verifiable();

        shareDirectoryClientMock
            .Setup(x => x.GetFileClient(It.IsAny<string>()))
            .Callback<string>(t => Assert.Equal(filename, t))
            .Returns(shareFileClientMock.Object)
            .Verifiable();

        shareFileClientMock
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>()))
            .Callback<ShareFileRequestConditions, CancellationToken>((c, t) =>
            {
                Assert.Equal(cancellationToken, t);
            })
            .ReturnsAsync(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()))
            .Verifiable();

        var provider = new AzureFileProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.DeleteAsync(this.filename, this.target, cancellationToken);

        // Assert
        shareClientMock.Verify(x => x.GetDirectoryClient(target), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        shareDirectoryClientMock.Verify(x => x.GetFileClient(filename), Times.Once);
        shareFileClientMock.Verify(x => x.DeleteIfExistsAsync(It.IsAny<ShareFileRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.Success);
        Assert.Equal($"The file {filename} does not exist in the container {tenant}", result.Message);
    }

}
