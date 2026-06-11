using CodeDesignPlus.Net.gRpc.Clients.Services.Emails;
using CodeDesignPlus.Net.Microservice.Emails.gRpc;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Emails;

public class EmailServiceTest
{
    [Fact]
    public async Task RenderTemplateAsync_ShouldReturnResponse_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new RenderTemplateResponse
        {
            RenderedHtml = "<h1>Hello John</h1>",
            Subject = "Welcome John",
            Success = true,
            Error = string.Empty
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.RenderTemplateAsync(It.IsAny<RenderTemplateRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new RenderTemplateRequest { TemplateType = "PurchaseConfirmation" };
        request.Values.Add("buyer_name", "John Doe");

        // Act
        var response = await emailService.RenderTemplateAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.RenderTemplateAsync(It.IsAny<RenderTemplateRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("<h1>Hello John</h1>", response.RenderedHtml);
        Assert.Equal("Welcome John", response.Subject);
    }

    [Fact]
    public async Task GeneratePdfAsync_ShouldReturnFileReference_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var fileId = Guid.NewGuid().ToString();
        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new GeneratePdfResponse
        {
            Id = fileId,
            Name = "PurchaseReceipt-123.pdf",
            Target = "emails-pdf/tenant-id",
            SignedUrl = "https://storage.example.com/signed-url",
            Success = true,
            Error = string.Empty
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.GeneratePdfAsync(It.IsAny<GeneratePdfRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new GeneratePdfRequest { TemplateType = "PurchaseReceipt" };
        request.Values.Add("buyer_name", "John Doe");

        // Act
        var response = await emailService.GeneratePdfAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.GeneratePdfAsync(It.IsAny<GeneratePdfRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(fileId, response.Id);
        Assert.Equal("PurchaseReceipt-123.pdf", response.Name);
        Assert.Equal("emails-pdf/tenant-id", response.Target);
        Assert.Equal("https://storage.example.com/signed-url", response.SignedUrl);
    }
}
