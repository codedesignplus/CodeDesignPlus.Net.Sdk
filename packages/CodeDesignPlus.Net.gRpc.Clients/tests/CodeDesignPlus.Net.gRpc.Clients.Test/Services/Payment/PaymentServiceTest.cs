using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;
using PaymentProto = CodeDesignPlus.Net.gRpc.Clients.Services.Payment.Payment;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Payment;

public class PaymentServiceTest
{
    private static Mock<IUserContext> CreateUserContext(string? accessToken, Guid tenant)
    {
        var userContext = new Mock<IUserContext>();
        userContext.Setup(uc => uc.AccessToken).Returns(accessToken!);
        userContext.Setup(uc => uc.Tenant).Returns(tenant);

        return userContext;
    }

    private static InitiatePaymentRequest CreateInitiateRequest() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Module = "TestModule",
        ReferenceId = Guid.NewGuid().ToString(),
        Description = "Test Payment",
        Buyer = new Buyer
        {
            BuyerId = Guid.NewGuid().ToString(),
            Name = "Test User",
            Email = "test@example.com",
            Phone = "3001234567",
            TypeDocument = new TypeDocument { Code = "CC", Name = "Cédula de ciudadanía" },
            Document = "1234567890"
        },
        SubTotal = new Amount { Currency = "COP", Value = 10_000_00 },
        Tax = new Amount { Currency = "COP", Value = 0 },
        Total = new Amount { Currency = "COP", Value = 10_000_00 }
    };

    private static RequestPayoutRequest CreatePayoutRequest(string tenant) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Tenant = tenant,
        Module = "Expenses",
        ReferenceId = Guid.NewGuid().ToString(),
        Purpose = PayoutPurpose.Expense,
        BeneficiaryId = Guid.NewGuid().ToString(),
        Amount = new Amount { Currency = "COP", Value = 250_000_00 },
        Description = "Monthly cleaning service",
        RequestedBy = Guid.NewGuid().ToString()
    };

    [Fact]
    public async Task InitiatePaymentAsync_ValidRequest_ReturnsRedirect()
    {
        // Arrange
        var userContext = CreateUserContext("test-access-token", Guid.NewGuid());

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new InitiatePaymentResponse
            {
                PaymentId = Guid.NewGuid().ToString(),
                Success = true,
                NextAction = NextActionType.Redirect,
                RedirectUrl = "https://checkout.example.com/p/"
            }));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        var response = await paymentService.InitiatePaymentAsync(CreateInitiateRequest(), CancellationToken.None);

        // Assert
        Assert.Equal("https://checkout.example.com/p/", response.RedirectUrl);
    }

    [Fact]
    public async Task InitiatePaymentAsync_WithAccessToken_ForwardsBearerAndTenant()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var userContext = CreateUserContext("user-token", tenant);

        Grpc.Core.Metadata? headers = null;

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<InitiatePaymentRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new InitiatePaymentResponse()));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        await paymentService.InitiatePaymentAsync(CreateInitiateRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "Bearer user-token", tenant.ToString() }, new[] { headers!.GetValue("authorization"), headers!.GetValue("x-tenant") });
    }

    /// <summary>
    /// Without a user token the call goes out with no Authorization header, instead of an empty "Bearer " that the
    /// server rejects as an invalid token.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetPaymentStatusAsync_WithoutAccessToken_SendsNoAuthorizationHeader(string? accessToken)
    {
        // Arrange
        var userContext = CreateUserContext(accessToken, Guid.NewGuid());

        Grpc.Core.Metadata? headers = null;

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.GetPaymentStatusAsync(It.IsAny<GetPaymentStatusRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<GetPaymentStatusRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new GetPaymentStatusResponse { Status = PaymentStatus.Succeeded }));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        await paymentService.GetPaymentStatusAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(headers!.Get("authorization"));
    }

    /// <summary>
    /// A worker has no current tenant: the header is the tenant written in the request, not the context's.
    /// </summary>
    [Fact]
    public async Task RequestPayoutAsync_FromWorker_SendsTenantOfTheRequestWithoutAuthorization()
    {
        // Arrange
        var tenant = Guid.NewGuid().ToString();
        var userContext = CreateUserContext(null, Guid.Empty);

        Grpc.Core.Metadata? headers = null;

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.RequestPayoutAsync(It.IsAny<RequestPayoutRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<RequestPayoutRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new RequestPayoutResponse { Status = PayoutStatus.Requested }));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        await paymentService.RequestPayoutAsync(CreatePayoutRequest(tenant), CancellationToken.None);

        // Assert
        Assert.Equal(new string?[] { null, tenant }, new[] { headers!.Get("authorization")?.Value, headers!.GetValue("x-tenant") });
    }

    [Fact]
    public async Task RequestPayoutAsync_ValidRequest_SendsRequestAsIsAndReturnsResponse()
    {
        // Arrange
        var request = CreatePayoutRequest(Guid.NewGuid().ToString());
        var expected = new RequestPayoutResponse { PayoutId = request.Id, Status = PayoutStatus.AwaitingApproval };
        var userContext = CreateUserContext("user-token", Guid.NewGuid());

        RequestPayoutRequest? sent = null;

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.RequestPayoutAsync(It.IsAny<RequestPayoutRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<RequestPayoutRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((r, _, _, _) => sent = r)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(expected));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        var response = await paymentService.RequestPayoutAsync(request, CancellationToken.None);

        // Assert
        Assert.Same(request, sent);
        Assert.Same(expected, response);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task RequestPayoutAsync_WithoutTenant_ThrowsArgumentException(string tenant)
    {
        // Arrange
        var paymentService = new PaymentService(new Mock<PaymentProto.PaymentClient>().Object, CreateUserContext(null, Guid.NewGuid()).Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => paymentService.RequestPayoutAsync(CreatePayoutRequest(tenant), CancellationToken.None));
    }

    [Fact]
    public async Task GetPayoutAsync_FromWorker_SendsTenantOfTheRequest()
    {
        // Arrange
        var tenant = Guid.NewGuid().ToString();
        var userContext = CreateUserContext(null, Guid.Empty);

        Grpc.Core.Metadata? headers = null;

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.GetPayoutAsync(It.IsAny<GetPayoutRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<GetPayoutRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new GetPayoutResponse { Status = PayoutStatus.Completed }));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        await paymentService.GetPayoutAsync(new GetPayoutRequest { Id = Guid.NewGuid().ToString(), Tenant = tenant }, CancellationToken.None);

        // Assert
        Assert.Equal(tenant, headers!.GetValue("x-tenant"));
    }

    [Fact]
    public async Task GetMerchantAccountAsync_FromWorker_SendsTenantOfTheRequest()
    {
        // Arrange
        var tenant = Guid.NewGuid().ToString();
        var userContext = CreateUserContext(null, Guid.Empty);

        Grpc.Core.Metadata? headers = null;

        var mockClient = new Mock<PaymentProto.PaymentClient>();
        mockClient
            .Setup(m => m.GetMerchantAccountAsync(It.IsAny<GetMerchantAccountRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Callback<GetMerchantAccountRequest, Grpc.Core.Metadata, DateTime?, CancellationToken>((_, h, _, _) => headers = h)
            .Returns(GrpcUtil.CreateAsyncUnaryCall(new GetMerchantAccountResponse { Status = MerchantAccountStatus.Active }));

        var paymentService = new PaymentService(mockClient.Object, userContext.Object);

        // Act
        await paymentService.GetMerchantAccountAsync(new GetMerchantAccountRequest { Tenant = tenant }, CancellationToken.None);

        // Assert
        Assert.Equal(tenant, headers!.GetValue("x-tenant"));
    }

    [Fact]
    public async Task GetMerchantAccountAsync_WithoutTenant_ThrowsArgumentException()
    {
        // Arrange
        var paymentService = new PaymentService(new Mock<PaymentProto.PaymentClient>().Object, CreateUserContext("user-token", Guid.NewGuid()).Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => paymentService.GetMerchantAccountAsync(new GetMerchantAccountRequest(), CancellationToken.None));
    }
}
