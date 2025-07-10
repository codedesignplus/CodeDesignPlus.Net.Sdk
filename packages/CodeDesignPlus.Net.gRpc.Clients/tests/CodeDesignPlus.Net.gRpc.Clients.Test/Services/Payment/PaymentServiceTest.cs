using System;
using CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;
using CodeDesignPlus.Net.gRpc.Clients.Extensions;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Ductus.FluentDocker.Commands;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Payment;

public class PaymentServiceTest
{
    [Fact]
    public async Task InitiatePaymentAsync_ShouldReturnResponse_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new InitiatePaymentResponse
        {
            Message = "Payment initiated successfully",
            TransactionId = Guid.NewGuid().ToString(),
            RedirectUrl = "https://example.com/redirect",
            Status = PaymentStatus.Pending,
            Success = true,
            FinancialNetwork = new FinancialNetwork
            {
                PaymentNetworkResponseCode = "00",
                PaymentNetworkResponseErrorMessage = "Success",
                TrazabilityCode = "TRAZ1234567890",
                AuthorizationCode = "AUTH123456",
                ResponseCode = "00",
            }
        });


        var mockClient = new Mock<gRpc.Clients.Services.Payment.Payment.PaymentClient>();
        mockClient
            .Setup(m => m.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var paymentService = new PaymentService(mockClient.Object, userContextMock.Object);
        var request = new InitiatePaymentRequest
        {
            Description = "Test Payment",
            Id = Guid.NewGuid().ToString(),
            Module = "TestModule",
            Payer = new Payer
            {
                BillingAddress = new Address
                {
                    City = "Test City",
                    Country = "Test Country",
                    Street = "123 Test St",
                    PostalCode = "12345",
                    State = "Test State",
                    Phone = "123-456-7890"
                },
            },
            PaymentMethod = new PaymentMethod
            {
                Type = "creditCard",
                CreditCard = new CreditCard
                {
                    Number = "4111111111111111",
                    ExpirationDate = "2025/12",
                    SecurityCode = "123",
                    InstallmentsNumber = 1,
                    Name = "Test User",
                }
            },
            Provider = PaymentProvider.Payu,
            SubTotal = new Amount()
            {
                Currency = "USD",
                Value = 100,
            },
            Total = new Amount()
            {
                Currency = "USD",
                Value = 100,
            },
            Tax = new Amount()
            {
                Currency = "USD",
                Value = 0,
            },
        };

        // Act
        var response = await paymentService.InitiatePaymentAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockClient.Verify(c => c.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnResponse_WhenCalledWithValidId()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new UpdateStatusResponse
        {
            Message = "Payment status updated successfully",
            Success = true,
        });

        var mockClient = new Mock<gRpc.Clients.Services.Payment.Payment.PaymentClient>();
        mockClient
            .Setup(m => m.UpdateStatusAsync(It.IsAny<UpdateStatusRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var paymentService = new PaymentService(mockClient.Object, userContextMock.Object);
        var paymentId = Guid.NewGuid();

        // Act
        var response = await paymentService.UpdateStatusAsync(paymentId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        mockClient.Verify(c => c.UpdateStatusAsync(It.IsAny<UpdateStatusRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}