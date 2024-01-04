using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Factories;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test;

public class AzureBlobProvider1Test
{
    private CancellationTokenSource cancellationTokenSource;
    private readonly CancellationToken cancellationToken;
    private readonly Guid tenant;
    private readonly string filename;
    private readonly string path;
    private readonly FileStream stream;
    private readonly string target;
    private readonly string blobname;
    private readonly Abstractions.Models.Path pathDetail;
    private readonly Abstractions.Models.File file;
    private readonly Mock<ILogger<AzureBlobProvider<Guid, Guid>>> loggerMock;
    private readonly Mock<IHostEnvironment> environmentMock;
    private readonly IOptions<FileStorageOptions> options;
    private readonly Mock<BlobContainerClient> blobContainerClientMock;
    private readonly Mock<BlobClient> blobClientMock;
    private readonly Mock<IAzureBlobFactory<Guid, Guid>> factoryMock;
    private readonly Mock<IUserContext<Guid, Guid>> userContextMock;

    public AzureBlobProvider1Test()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = cancellationTokenSource.Token;
        this.tenant = Guid.NewGuid();
        this.filename = "FakeDocument.txt";
        this.path = Path.Combine(AppContext.BaseDirectory, "Helpers", "Files", filename);
        this.stream = System.IO.File.OpenRead(path);
        this.target = "docs/general";
        this.blobname = $"{target}/{filename}";
        this.pathDetail = new Abstractions.Models.Path(OptionsUtil.FileStorageOptions.UriDownload, target, this.blobname, TypeProviders.AzureBlobProvider);
        this.file = new Abstractions.Models.File(filename);
        this.options = O.Options.Create(OptionsUtil.FileStorageOptions);

        this.loggerMock = new Mock<ILogger<AzureBlobProvider<Guid, Guid>>>();
        this.userContextMock = new Mock<IUserContext<Guid, Guid>>();
        this.environmentMock = new Mock<IHostEnvironment>();
        this.blobContainerClientMock = new Mock<BlobContainerClient>();
        this.blobClientMock = new Mock<BlobClient>();
        this.factoryMock = new Mock<IAzureBlobFactory<Guid, Guid>>();

        userContextMock.SetupGet(x => x.Tenant).Returns(tenant);

        factoryMock.SetupGet(x => x.Options).Returns(options.Value);
        factoryMock.SetupGet(x => x.UserContext).Returns(userContextMock.Object);

        factoryMock.Setup(x => x.GetContainerClient()).Returns(blobContainerClientMock.Object).Verifiable();
        factoryMock.Setup(x => x.Create()).Returns(factoryMock.Object).Verifiable();

        blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object).Verifiable();
    }

    [Fact]
    public async void UploadAsync()
    {
        // Arrange        
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

        var provider = new AzureBlobProvider<Guid, Guid>(factoryMock.Object, loggerMock.Object, environmentMock.Object);

        // Act
        var result = await provider.UploadAsync(stream, file, target, cancellationToken);

        // Assert
        blobContainerClientMock.Verify(x => x.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<BlobContainerEncryptionScopeOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        blobContainerClientMock.Verify(x => x.GetBlobClient(blobname), Times.Once);
        blobClientMock.Verify(x => x.UploadAsync(stream, It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        factoryMock.Verify(x => x.GetContainerClient(), Times.Once);

        Assert.True(result.Success);
        Assert.Equal(stream.Length, result.File.Size);

        Assert.Equal(pathDetail.Target, result.File.Path.Target);
        Assert.Equal(pathDetail.File, result.File.Path.File);
        Assert.Equal(pathDetail.Provider, result.File.Path.Provider);
        Assert.Equal(pathDetail.UriDownload, result.File.Path.UriDownload);
        Assert.Equal(pathDetail.UriViewInBrowser, result.File.Path.UriViewInBrowser);
        Assert.Equal(pathDetail.Uri, result.File.Path.Uri);
    }

}
