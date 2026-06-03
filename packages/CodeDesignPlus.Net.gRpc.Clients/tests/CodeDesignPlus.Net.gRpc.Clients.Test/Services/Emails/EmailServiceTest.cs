using CodeDesignPlus.Net.gRpc.Clients.Services.Emails;
using CodeDesignPlus.Net.Microservice.Emails.gRpc;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Google.Protobuf;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Emails;

public class EmailServiceTest
{
    [Fact]
    public async Task SendEmailAsync_ShouldReturnResponse_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new SendEmailResponse
        {
            Code = "200",
            Message = "Email sent successfully"
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new SendEmailRequest
        {
            Id = Guid.NewGuid().ToString(),
            IdTemplate = Guid.NewGuid().ToString(),
            Subject = "Test Email",
            Body = "<h1>Test Body</h1>",
            From = "test@example.com"
        };
        request.To.Add("recipient@example.com");
        request.Values.Add("Name", "John Doe");

        // Act
        var response = await emailService.SendEmailAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(response);
        Assert.Equal("200", response.Code);
        Assert.Equal("Email sent successfully", response.Message);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldIncludeAttachments_WhenProvidedInRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new SendEmailResponse
        {
            Code = "200",
            Message = "Email with attachment sent successfully"
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new SendEmailRequest
        {
            Id = Guid.NewGuid().ToString(),
            Subject = "Test Email with Attachment",
            Body = "<h1>Test Body</h1>",
            From = "test@example.com"
        };
        request.To.Add("recipient@example.com");
        request.Attachments.Add(new Attachment
        {
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Content = ByteString.CopyFrom(new byte[] { 0x25, 0x50, 0x44, 0x46 }) // PDF magic bytes
        });

        // Act
        var response = await emailService.SendEmailAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.SendEmailAsync(
            It.Is<SendEmailRequest>(r => r.Attachments.Count == 1),
            It.IsAny<Grpc.Core.Metadata>(),
            It.IsAny<DateTime?>(),
            It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(response);
        Assert.Equal("200", response.Code);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldIncludeCcAndBcc_WhenProvidedInRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new SendEmailResponse
        {
            Code = "200",
            Message = "Email sent successfully"
        });

        var mockClient = new Mock<CodeDesignPlus.Net.Microservice.Emails.gRpc.Emails.EmailsClient>();
        mockClient
            .Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var emailService = new EmailService(mockClient.Object, userContextMock.Object);
        var request = new SendEmailRequest
        {
            Id = Guid.NewGuid().ToString(),
            Subject = "Test Email",
            Body = "<h1>Test Body</h1>",
            From = "test@example.com"
        };
        request.To.Add("recipient@example.com");
        request.Cc.Add("cc@example.com");
        request.Bcc.Add("bcc@example.com");

        // Act
        var response = await emailService.SendEmailAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(c => c.SendEmailAsync(
            It.Is<SendEmailRequest>(r =>
                r.To.Count == 1 &&
                r.Cc.Count == 1 &&
                r.Bcc.Count == 1),
            It.IsAny<Grpc.Core.Metadata>(),
            It.IsAny<DateTime?>(),
            It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(response);
        Assert.Equal("200", response.Code);
    }
}
