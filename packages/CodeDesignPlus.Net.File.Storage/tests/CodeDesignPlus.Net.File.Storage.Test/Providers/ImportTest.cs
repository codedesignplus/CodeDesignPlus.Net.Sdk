using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Providers;
using Microsoft.Extensions.Hosting;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Providers;

/// <summary>
/// Importar copia en el servidor un archivo que otro sistema dejó en un contenedor propio (una grabación) a la carpeta de
/// la copropiedad, sin pasar su contenido por el proceso, y deja el origen hasta que se pida borrarlo.
/// </summary>
public class ImportTest
{
    private const string SourceContainer = "acs-recordings";
    private const string SourcePath = "20261021/call/recording/0-mixed.mp4";
    private const string FileName = "session-1-part-1.mp4";
    private const string Target = "assembly-recordings/2b1f3f5e-0000-0000-0000-000000000001";

    private readonly Guid tenant = Guid.NewGuid();
    private readonly Mock<IAzureBlobFactory> factory = new();
    private readonly Mock<BlobServiceClient> service = new();
    private readonly Mock<BlobContainerClient> sourceContainer = new();
    private readonly Mock<BlobClient> source = new();
    private readonly Mock<BlobContainerClient> tenantContainer = new();
    private readonly Mock<BlobClient> destination = new();
    private readonly Mock<CopyFromUriOperation> copy = new();

    public ImportTest()
    {
        factory.SetupGet(x => x.Options).Returns(OptionsUtil.FileStorageOptions);
        factory.SetupGet(x => x.Client).Returns(service.Object);
        factory.Setup(x => x.Create()).Returns(factory.Object);
        factory.Setup(x => x.GetContainerClient(tenant)).Returns(tenantContainer.Object);

        service.Setup(x => x.GetBlobContainerClient(SourceContainer)).Returns(sourceContainer.Object);
        sourceContainer.Setup(x => x.GetBlobClient(SourcePath)).Returns(source.Object);
        source.SetupGet(x => x.Uri).Returns(new Uri($"https://account.blob.core.windows.net/{SourceContainer}/{SourcePath}"));
        source.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        tenantContainer.Setup(x => x.GetBlobClient($"{Target}/{FileName}")).Returns(destination.Object);

        destination
            .Setup(x => x.StartCopyFromUriAsync(It.IsAny<Uri>(), It.IsAny<BlobCopyFromUriOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(copy.Object);

        copy.Setup(x => x.WaitForCompletionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(10L, Mock.Of<Response>()));

        SetCopyStatus(CopyStatus.Success);
    }

    private void SetCopyStatus(CopyStatus status)
    {
        // BlobsModelFactory.BlobProperties pide sus 44 argumentos: se invoca por reflexión dando solo los que lee la importación.
        var factoryMethod = typeof(BlobsModelFactory).GetMethods()
            .Where(x => x.Name == nameof(BlobsModelFactory.BlobProperties))
            .OrderByDescending(x => x.GetParameters().Length)
            .First();

        var values = new Dictionary<string, object?>
        {
            ["contentLength"] = 2048L,
            ["blobCopyStatus"] = status,
            ["contentType"] = "application/octet-stream",
        };

        var names = factoryMethod.GetParameters().Select(x => x.Name!).ToList();

        Assert.True(values.Keys.All(names.Contains), string.Join(", ", names));

        var arguments = factoryMethod.GetParameters()
            .Select(x => values.TryGetValue(x.Name!, out var value) ? value : x.ParameterType.IsValueType ? Activator.CreateInstance(x.ParameterType) : null)
            .ToArray();

        var properties = (BlobProperties)factoryMethod.Invoke(null, arguments)!;

        destination
            .Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(properties, Mock.Of<Response>()));
    }

    private AzureBlobProvider Blob() => new(factory.Object, new Mock<ILogger<AzureBlobProvider>>().Object, new Mock<IHostEnvironment>().Object);

    [Fact]
    public async Task ImportAsync_BlobSourceExists_CopiesOnTheServerIntoTheTenantFolder()
    {
        // Arrange
        var provider = Blob();

        // Act
        var response = await provider.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant);

        // Assert
        Assert.True(response.Success, response.Message);
        Assert.Equal(2048, response.File.Size);
        Assert.Equal(FileName, response.File.Detail.File);
        Assert.Equal(Target, response.File.Detail.Target);
        destination.Verify(x => x.StartCopyFromUriAsync(source.Object.Uri, It.IsAny<BlobCopyFromUriOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        destination.Verify(x => x.SetHttpHeadersAsync(It.Is<BlobHttpHeaders>(h => h.ContentType == "video/mp4"), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
        source.Verify(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_BlobSourceMissing_FailsWithoutCopying()
    {
        // Arrange
        source.Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        var provider = Blob();

        // Act
        var response = await provider.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant);

        // Assert
        Assert.False(response.Success);
        destination.Verify(x => x.StartCopyFromUriAsync(It.IsAny<Uri>(), It.IsAny<BlobCopyFromUriOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ImportAsync_BlobCopyAborted_Fails()
    {
        // Arrange
        SetCopyStatus(CopyStatus.Aborted);

        var provider = Blob();

        // Act
        var response = await provider.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant);

        // Assert
        Assert.False(response.Success);
    }

    /// <summary>
    /// Si Azure rechaza la copia, la respuesta dice el tipo, el estado y el código del error en una línea, sin las cabeceras
    /// que Azure añade al mensaje: es lo que ms-filestorage devuelve a quien pidió importar.
    /// </summary>
    [Fact]
    public async Task ImportAsync_StorageRejectsTheCopy_SaysWhyInOneLine()
    {
        // Arrange
        destination
            .Setup(x => x.StartCopyFromUriAsync(It.IsAny<Uri>(), It.IsAny<BlobCopyFromUriOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(403, "This request is not authorized to perform this operation.\nRequestId:1\nHeaders:\nServer: Windows-Azure-Blob/1.0", "AuthorizationPermissionMismatch", null));

        var provider = Blob();

        // Act
        var response = await provider.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("RequestFailedException (403 AuthorizationPermissionMismatch): This request is not authorized to perform this operation.", response.Message);
    }

    [Fact]
    public async Task DeleteImportSourceAsync_Blob_DeletesTheSource()
    {
        // Arrange
        source
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        var provider = Blob();

        // Act
        var response = await provider.DeleteImportSourceAsync(SourceContainer, SourcePath);

        // Assert
        Assert.True(response.Success);
        source.Verify(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteImportSourceAsync_BlobAlreadyGone_Succeeds()
    {
        // Arrange
        source
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        var provider = Blob();

        // Act
        var response = await provider.DeleteImportSourceAsync(SourceContainer, SourcePath);

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task ImportAsync_Local_CopiesAndKeepsTheSourceUntilDeleted()
    {
        // Arrange
        var options = OptionsUtil.FileStorageOptions;
        var provider = new LocalProvider(O.Options.Create(options), new Mock<ILogger<LocalProvider>>().Object, new Mock<IHostEnvironment>().Object);
        var container = $"imports-{Guid.NewGuid():N}";
        var sourceFile = Path.Combine(options.Local.Folder, container, "recording.mp4");

        Directory.CreateDirectory(Path.GetDirectoryName(sourceFile)!);
        await System.IO.File.WriteAllTextAsync(sourceFile, "video");

        // Act
        var imported = await provider.ImportAsync(container, "recording.mp4", FileName, Target, tenant);
        var sourceKept = System.IO.File.Exists(sourceFile);
        var released = await provider.DeleteImportSourceAsync(container, "recording.mp4");

        // Assert
        Assert.True(imported.Success);
        Assert.True(sourceKept);
        Assert.True(released.Success);
        Assert.False(System.IO.File.Exists(sourceFile));
        Assert.Equal("video", await System.IO.File.ReadAllTextAsync(Path.Combine(options.Local.Folder, tenant.ToString(), Target, FileName)));
    }

    [Fact]
    public async Task ImportAsync_Service_AsksEveryProviderAndLeavesOutTheDisabled()
    {
        // Arrange
        var enabled = new Mock<IAzureBlobProvider>().As<IProvider>();
        var disabled = new Mock<ILocalProvider>().As<IProvider>();

        enabled
            .Setup(x => x.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M.Response(new M.File(FileName), TypeProviders.AzureBlobProvider) { Success = true });

        disabled
            .Setup(x => x.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant, It.IsAny<CancellationToken>()))
            .ReturnsAsync((M.Response)null!);

        var service = new FileStorageService([enabled.Object, disabled.Object]);

        // Act
        var responses = await service.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant);

        // Assert
        Assert.Single(responses);
        disabled.Verify(x => x.ImportAsync(SourceContainer, SourcePath, FileName, Target, tenant, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", SourcePath)]
    [InlineData(SourceContainer, "")]
    [InlineData(SourceContainer, "../2b1f3f5e-0000-0000-0000-000000000001/minutes.pdf")]
    [InlineData(SourceContainer, "recordings\\..\\..\\secret.mp4")]
    public async Task ImportAsync_Service_InvalidSource_Throws(string container, string path)
    {
        // Arrange
        var service = new FileStorageService([]);

        // Act & Assert
        await Assert.ThrowsAsync<FileStorageException>(() => service.ImportAsync(container, path, FileName, Target, tenant));
    }

    [Fact]
    public async Task ImportAsync_Service_EmptyTenant_Throws()
    {
        // Arrange
        var service = new FileStorageService([]);

        // Act & Assert
        await Assert.ThrowsAsync<FileStorageException>(() => service.ImportAsync(SourceContainer, SourcePath, FileName, Target, Guid.Empty));
    }

    [Fact]
    public async Task DeleteImportSourceAsync_Service_PathOutsideTheContainer_Throws()
    {
        // Arrange
        var service = new FileStorageService([]);

        // Act & Assert
        await Assert.ThrowsAsync<FileStorageException>(() => service.DeleteImportSourceAsync(SourceContainer, "../x.mp4"));
    }
}
