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
            Subject = "Welcome",
            Success = true,
            Error = string.Empty
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.RenderTemplateAsync(It.IsAny<RenderTemplateRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new RenderTemplateRequest
        {
            TemplateType = "WelcomeEmail"
        };
        request.Values.Add("Name", "John Doe");

        // Act
        var response = await emailService.RenderTemplateAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.RenderTemplateAsync(It.IsAny<RenderTemplateRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("<h1>Hello John</h1>", response.RenderedHtml);
        Assert.Equal("Welcome", response.Subject);
    }

    [Fact]
    public async Task GeneratePdfAsync_ShouldReturnResponse_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 };

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new GeneratePdfResponse
        {
            PdfContent = Google.Protobuf.ByteString.CopyFrom(pdfBytes),
            Success = true,
            Error = string.Empty
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.GeneratePdfAsync(It.IsAny<GeneratePdfRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var tenant = Guid.NewGuid();
        var request = new GeneratePdfRequest
        {
            TemplateType = "Invoice",
            Tenant = tenant.ToString()
        };
        request.Values.Add("OrderId", "12345");
        request.Values.Add("Total", "100.00");

        // Act
        var response = await emailService.GeneratePdfAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.GeneratePdfAsync(It.IsAny<GeneratePdfRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(pdfBytes, response.PdfContent.ToByteArray());
    }

    [Fact]
    public async Task GeneratePdfAsync_ShouldReturnError_WhenGenerationFails()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new GeneratePdfResponse
        {
            Success = false,
            Error = "Template not found"
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.GeneratePdfAsync(It.IsAny<GeneratePdfRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new GeneratePdfRequest
        {
            TemplateType = "NonExistentTemplate",
            Tenant = Guid.NewGuid().ToString()
        };

        // Act
        var response = await emailService.GeneratePdfAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Template not found", response.Error);
    }
}
