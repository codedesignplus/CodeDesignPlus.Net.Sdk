using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Providers;

public class AzureBlobProviderTest
{
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly CancellationToken cancellationToken;
    private readonly Guid tenant;
    private readonly string filename;
    private readonly string path;
    private readonly FileStream stream;
    private string target;
    private string blobname;
    private M.FileDetail pathDetail;
    private readonly M.File file;
    private readonly Mock<ILogger<AzureBlobProvider>> loggerMock;
    private readonly Mock<IHostEnvironment> environmentMock;
    private readonly IOptions<FileStorageOptions> options;
    private readonly Mock<BlobContainerClient> blobContainerClientMock;
    private readonly Mock<BlobClient> blobClientMock;
    private readonly Mock<IAzureBlobFactory> factoryMock;
    private readonly Mock<IUserContext> userContextMock;

    public AzureBlobProviderTest()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = cancellationTokenSource.Token;
        this.tenant = Guid.NewGuid();
        this.filename = "FakeDocument.txt";
        this.path = Path.Combine(AppContext.BaseDirectory, "Helpers", "Files", filename);
        this.stream = System.IO.File.OpenRead(path);
        this.target = "docs/general";
        this.blobname = $"{target}/{filename}";
        this.pathDetail = new M.FileDetail(OptionsUtil.FileStorageOptions.UriDownload, target, this.filename, TypeProviders.AzureBlobProvider);
        this.file = new M.File(filename);
        this.options = O.Options.Create(OptionsUtil.FileStorageOptions);

        this.loggerMock = new Mock<ILogger<AzureBlobProvider>>();
        this.userContextMock = new Mock<IUserContext>();
        this.environmentMock = new Mock<IHostEnvironment>();
        this.blobContainerClientMock = new Mock<BlobContainerClient>();
        this.blobClientMock = new Mock<BlobClient>();
        this.factoryMock = new Mock<IAzureBlobFactory>();

        userContextMock.SetupGet(x => x.Tenant).Returns(tenant);

        factoryMock.SetupGet(x => x.Options).Returns(options.Value);
        factoryMock.SetupGet(x => x.UserContext).Returns(userContextMock.Object);

        factoryMock.Setup(x => x.GetContainerClient()).Returns(blobContainerClientMock.Object).Verifiable();
        factoryMock.Setup(x => x.Create()).Returns(factoryMock.Object).Verifiable();

        blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object).Verifiable();

        blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, BlobUploadOptions, CancellationToken>((s, options, t) =>
            {
                Assert.Equal(stream, s);
                Assert.Equal(AccessTier.Hot, options.AccessTier);
                Assert.Contains(file.GetMetadata(pathDetail.Uri), x => options.Metadata.Any(o => o.Key == x.Key));
                Assert.Contains(file.GetTags(tenant), x => options.Tags.Any(o => o.Key == x.Key));
                Assert.Equal(file.Mime.MimeType, options.HttpHeaders.ContentType);
                Assert.Equal(cancellationToken, t);
            });

        blobClientMock
            .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((s, t) =>
            {
                stream.Position = 0;
                stream.CopyTo(s);
                Assert.Equal(stream.Length, s.Length);
                Assert.Equal(cancellationToken, t);
            });

        blobClientMock
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .Callback<DeleteSnapshotsOption, BlobRequestConditions, CancellationToken>((d, b, t) =>
            {
                Assert.Equal(cancellationToken, t);
            })
            .ReturnsAsync(Azure.Response.FromValue(true, Mock.Of<Azure.Response>()))
            .Verifiable();
    }

    [Fact]
    public async Task UploadAsync_Default_Success()
    {
        // Arrange
        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, cancellationToken: cancellationToken);

        // Assert
        this.AssertsUpload(result);
    }

    [Fact]
    public async Task UploadAsync_EmptyTarget_Success()
    {
        // Arrange     
        this.target = null!;
        this.blobname = $"{filename}";
        this.pathDetail = new M.FileDetail(OptionsUtil.FileStorageOptions.UriDownload, target, this.blobname, TypeProviders.AzureBlobProvider);

        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, cancellationToken: cancellationToken);

        // Assert
        this.AssertsUpload(result);
    }


    [Fact]
    public async Task UploadAsync_Renowned_Success()
    {
        // Arrange    
        file.Renowned = true;
        this.blobname = $"{target}/{file.Name} ({2}){file.Extension}";
        this.pathDetail = new M.FileDetail(OptionsUtil.FileStorageOptions.UriDownload, target, System.IO.Path.GetFileName(this.blobname), TypeProviders.AzureBlobProvider);

        var existContainer = true;
        blobClientMock
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

        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, this.filename, target, true, cancellationToken);

        // Assert
        this.AssertsUpload(result, 2);
    }

    private void AssertsUpload(M.Response result, int major = 1)
    {
        blobContainerClientMock.Verify(x => x.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<BlobContainerEncryptionScopeOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        blobContainerClientMock.Verify(x => x.GetBlobClient(blobname), Times.Once);
        blobClientMock.Verify(x => x.UploadAsync(stream, It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);

        Assert.True(result.Success);
        Assert.Equal(stream.Length, result.File.Size);
        Assert.Equal(file.Renowned, result.File.Renowned);
        Assert.Equal(major, result.File.Version.Major);

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
        blobClientMock
          .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
          .Callback<CancellationToken>(t =>
          {
              Assert.Equal(cancellationToken, t);
          })
          .ReturnsAsync(Azure.Response.FromValue(true, Mock.Of<Azure.Response>()))
          .Verifiable();

        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);


        // Act
        var result = await provider.DownloadAsync(filename, target, cancellationToken);

        // Assert
        blobContainerClientMock.Verify(x => x.GetBlobClient(blobname), Times.Once);
        blobClientMock.Verify(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        blobClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);

        this.stream.Position = 0;
        Assert.True(result.Success);
        Assert.Equal(stream.Length, result.Stream.Length);
        Assert.Equal(0, result.Stream.Position);
        Assert.True(Helpers.Extensions.CompareStreams(stream, result.Stream));
    }

    [Fact]
    public async Task DownloadAsync_FileNotExist_Success()
    {
        // Arrange
        blobClientMock
          .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
          .Callback<CancellationToken>(t =>
          {
              Assert.Equal(cancellationToken, t);
          })
          .ReturnsAsync(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()))
          .Verifiable();

        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);


        // Act
        var result = await provider.DownloadAsync(filename, target, cancellationToken);

        // Assert
        blobContainerClientMock.Verify(x => x.GetBlobClient(blobname), Times.Once);
        blobClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        blobClientMock.Verify(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.False(result.Success);
        Assert.Equal($"The file {filename} not exist in the container {tenant}", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_FileExist_Success()
    {
        // Arrange

        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.DeleteAsync(filename, target, cancellationToken);

        // Arrange
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        blobContainerClientMock.Verify(x => x.GetBlobClient(blobname), Times.Once);
        blobClientMock.Verify(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task DeleteAsync_FileNotExist_Success()
    {
        // Arrange
        blobClientMock
          .Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
          .Callback<DeleteSnapshotsOption, BlobRequestConditions, CancellationToken>((d, b, t) =>
          {
              Assert.Equal(cancellationToken, t);
          })
          .ReturnsAsync(Azure.Response.FromValue(false, Mock.Of<Azure.Response>()))
          .Verifiable();

        var provider = new AzureBlobProvider(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.DeleteAsync(filename, target, cancellationToken);

        // Arrange
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);
        blobContainerClientMock.Verify(x => x.GetBlobClient(blobname), Times.Once);
        blobClientMock.Verify(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.Success);
        Assert.Equal($"The file {filename} not exist in the container {tenant}", result.Message);
    }
}
