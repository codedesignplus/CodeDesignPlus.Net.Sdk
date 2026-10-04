using CodeDesignPlus.Net.gRpc.Clients.Services.FileStorage;
using CodeDesignPlus.Net.xUnit.Extensions;
using Google.Protobuf;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.FileStorage;

public class FileStorageGrpcServiceTest
{
    private static UploadFileRequest CreateRequest(string tenant) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Tenant = tenant,
        UploadedBy = Guid.NewGuid().ToString(),
        Target = "invoices",
        FileName = "collection-notice.pdf",
        Content = ByteString.CopyFrom(0x25, 0x50, 0x44, 0x46)
    };

    /// <summary>
    /// El request llega al servidor tal cual lo armó quien llama, y la respuesta vuelve sin tocar.
    /// </summary>
    [Fact]
    public async Task UploadAsync_SendsRequestAsIs_AndReturnsResponse()
    {
        // Arrange
        var request = CreateRequest(Guid.NewGuid().ToString());
        var expected = new UploadFileResponse
        {
            Id = request.Id,
            Target = request.Target,
            FileName = request.FileName
        };

        UploadFileRequest? sent = null;

        var client = new Mock<Files.FilesClient>();
        client
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<UploadFileRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((r, _, _, _) => sent = r)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(expected));

        var service = new FileStorageGrpcService(client.Object);

        // Act
        var response = await service.UploadAsync(request, CancellationToken.None);

        // Assert
        Assert.Same(request, sent);
        Assert.Same(expected, response);
        client.Verify(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// El token de cancelación de quien llama es el que recibe la llamada gRPC.
    /// </summary>
    [Fact]
    public async Task UploadAsync_PropagatesCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        CancellationToken received = default;

        var client = new Mock<Files.FilesClient>();
        client
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<UploadFileRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, _, _, ct) => received = ct)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new UploadFileResponse()));

        var service = new FileStorageGrpcService(client.Object);

        // Act
        await service.UploadAsync(CreateRequest(Guid.NewGuid().ToString()), token);

        // Assert
        Assert.Equal(token, received);
    }

    /// <summary>
    /// La copropiedad sale del request, porque un job no tiene contexto HTTP, y no se envía una cabecera
    /// Authorization vacía.
    /// </summary>
    [Fact]
    public async Task UploadAsync_SendsTenantFromRequest_AndNoAuthorization()
    {
        // Arrange
        var tenant = Guid.NewGuid().ToString();
        Grpc.Core.Metadata? headers = null;

        var client = new Mock<Files.FilesClient>();
        client
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<UploadFileRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new UploadFileResponse()));

        var service = new FileStorageGrpcService(client.Object);

        // Act
        await service.UploadAsync(CreateRequest(tenant), CancellationToken.None);

        // Assert
        Assert.NotNull(headers);
        Assert.Equal(tenant, headers.GetValue("x-tenant"));
        Assert.Null(headers.Get("authorization"));
    }

    /// <summary>
    /// Sin copropiedad en el request no se inventa una cabecera X-Tenant vacía.
    /// </summary>
    [Fact]
    public async Task UploadAsync_OmitsTenantHeader_WhenRequestHasNoTenant()
    {
        // Arrange
        Grpc.Core.Metadata? headers = null;

        var client = new Mock<Files.FilesClient>();
        client
            .Setup(c => c.UploadAsync(It.IsAny<UploadFileRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<UploadFileRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new UploadFileResponse()));

        var service = new FileStorageGrpcService(client.Object);

        // Act
        await service.UploadAsync(CreateRequest(string.Empty), CancellationToken.None);

        // Assert
        Assert.NotNull(headers);
        Assert.Empty(headers);
    }

    [Fact]
    public async Task UploadAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var service = new FileStorageGrpcService(new Mock<Files.FilesClient>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.UploadAsync(null!, CancellationToken.None));
    }
}
