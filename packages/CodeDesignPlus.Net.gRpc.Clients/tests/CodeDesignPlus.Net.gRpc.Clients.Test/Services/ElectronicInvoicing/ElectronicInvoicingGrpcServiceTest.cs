using CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;
using ElectronicInvoicingProto = CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing.ElectronicInvoicing;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.ElectronicInvoicing;

/// <summary>
/// The client of ms-electronicinvoicing is called by workers without a user: it sends the request as it comes and no
/// header, because the issuer is decided only by the request (Kappali pendings/661).
/// </summary>
public class ElectronicInvoicingGrpcServiceTest
{
    private static IssueDocumentRequest PlatformInvoice() => new()
    {
        Id = Guid.NewGuid().ToString(),
        PlatformIssuer = true,
        Type = DocumentType.Invoice,
        Reference = "license-order:" + Guid.NewGuid(),
        Content = new DocumentContent
        {
            Buyer = new Buyer { PersonTypeCode = "1", IdentificationTypeCode = "31", IdentificationNumber = "901888777", CheckDigit = "3", Name = "Conjunto Las Acacias" },
            Totals = new Totals { Currency = "COP", GrossAmount = 10_000_000, TaxAmount = 1_900_000, PayableAmount = 11_900_000 },
            PaymentMeans = new PaymentMeans { FormCode = "1", MethodCode = "48" },
            DueDate = "2026-11-10"
        }
    };

    [Fact]
    public async Task IssueDocumentAsync_PlatformInvoice_SendsTheRequestWithoutTenant()
    {
        // Arrange
        var request = PlatformInvoice();
        IssueDocumentRequest? sent = null;

        var client = new Mock<ElectronicInvoicingProto.ElectronicInvoicingClient>();
        client
            .Setup(m => m.IssueDocumentAsync(It.IsAny<IssueDocumentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<IssueDocumentRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((r, _, _, _) => sent = r)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new IssueDocumentResponse { Id = request.Id, Status = DocumentStatus.Draft }));

        var service = new ElectronicInvoicingGrpcService(client.Object);

        // Act
        var response = await service.IssueDocumentAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal((request.Id, DocumentStatus.Draft, true, false), (response.Id, response.Status, sent!.PlatformIssuer, sent.HasIssuerTenant));
    }

    /// <summary>
    /// Neither the caller's token nor its tenant travel: a header taken from the worker's context could only contradict
    /// the scope the request names.
    /// </summary>
    [Fact]
    public async Task IssueDocumentAsync_AnyCall_SendsNoHeaders()
    {
        // Arrange
        Grpc.Core.Metadata? headers = null;

        var client = new Mock<ElectronicInvoicingProto.ElectronicInvoicingClient>();
        client
            .Setup(m => m.IssueDocumentAsync(It.IsAny<IssueDocumentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<IssueDocumentRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new IssueDocumentResponse()));

        var service = new ElectronicInvoicingGrpcService(client.Object);

        // Act
        await service.IssueDocumentAsync(PlatformInvoice(), CancellationToken.None);

        // Assert
        Assert.Empty(headers!);
    }

    [Fact]
    public async Task IssueDocumentAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new ElectronicInvoicingGrpcService(Mock.Of<ElectronicInvoicingProto.ElectronicInvoicingClient>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.IssueDocumentAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Without an id the server would answer INVALID_ARGUMENT after a round trip; and an id is what makes a retry return
    /// the same document instead of issuing twice.
    /// </summary>
    [Fact]
    public async Task IssueDocumentAsync_WithoutId_ThrowsArgumentException()
    {
        // Arrange
        var request = PlatformInvoice();
        request.Id = string.Empty;

        var service = new ElectronicInvoicingGrpcService(Mock.Of<ElectronicInvoicingProto.ElectronicInvoicingClient>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.IssueDocumentAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task GetDocumentAsync_PlatformDocument_ReturnsTheDocument()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        var client = new Mock<ElectronicInvoicingProto.ElectronicInvoicingClient>();
        client
            .Setup(m => m.GetDocumentAsync(It.Is<GetDocumentRequest>(r => r.Id == id && r.PlatformIssuer && !r.HasTenant), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new GetDocumentResponse { Id = id, Status = DocumentStatus.Validated, Cufe = "cufe-1" }));

        var service = new ElectronicInvoicingGrpcService(client.Object);

        // Act
        var response = await service.GetDocumentAsync(new GetDocumentRequest { Id = id, PlatformIssuer = true }, CancellationToken.None);

        // Assert
        Assert.Equal((DocumentStatus.Validated, "cufe-1"), (response.Status, response.Cufe));
    }

    [Fact]
    public async Task GetDocumentAsync_WithoutId_ThrowsArgumentException()
    {
        // Arrange
        var service = new ElectronicInvoicingGrpcService(Mock.Of<ElectronicInvoicingProto.ElectronicInvoicingClient>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetDocumentAsync(new GetDocumentRequest { PlatformIssuer = true }, CancellationToken.None));
    }

    [Fact]
    public async Task RetryDocumentAsync_RejectedDocument_SendsTheCorrectedContent()
    {
        // Arrange
        var correction = PlatformInvoice().Content;
        var request = new RetryDocumentRequest { Id = Guid.NewGuid().ToString(), PlatformIssuer = true, Content = correction };
        RetryDocumentRequest? sent = null;

        var client = new Mock<ElectronicInvoicingProto.ElectronicInvoicingClient>();
        client
            .Setup(m => m.RetryDocumentAsync(It.IsAny<RetryDocumentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<RetryDocumentRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((r, _, _, _) => sent = r)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new IssueDocumentResponse { Id = request.Id, Status = DocumentStatus.Draft }));

        var service = new ElectronicInvoicingGrpcService(client.Object);

        // Act
        await service.RetryDocumentAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(correction, sent!.Content);
    }

    [Fact]
    public async Task RetryDocumentAsync_WithoutId_ThrowsArgumentException()
    {
        // Arrange
        var service = new ElectronicInvoicingGrpcService(Mock.Of<ElectronicInvoicingProto.ElectronicInvoicingClient>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.RetryDocumentAsync(new RetryDocumentRequest { PlatformIssuer = true }, CancellationToken.None));
    }

    [Fact]
    public async Task GetIssuerAsync_PlatformIssuer_ReturnsItsQuota()
    {
        // Arrange
        var client = new Mock<ElectronicInvoicingProto.ElectronicInvoicingClient>();
        client
            .Setup(m => m.GetIssuerAsync(It.Is<GetIssuerRequest>(r => r.PlatformIssuer && !r.HasTenant), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new GetIssuerResponse { Registered = true, CanIssue = true, Quota = new Quota { Remaining = 42 } }));

        var service = new ElectronicInvoicingGrpcService(client.Object);

        // Act
        var response = await service.GetIssuerAsync(new GetIssuerRequest { PlatformIssuer = true }, CancellationToken.None);

        // Assert
        Assert.Equal((true, 42L), (response.CanIssue, response.Quota.Remaining));
    }

    [Fact]
    public async Task GetIssuerAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new ElectronicInvoicingGrpcService(Mock.Of<ElectronicInvoicingProto.ElectronicInvoicingClient>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetIssuerAsync(null!, CancellationToken.None));
    }
}
