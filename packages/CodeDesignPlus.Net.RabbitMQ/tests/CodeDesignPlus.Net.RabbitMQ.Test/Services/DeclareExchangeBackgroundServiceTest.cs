using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

public class DeclareExchangeBackgroundServiceTest
{
    [Fact]
    public async Task ExecuteAsync_WithDomainEvents_CallsExchangeDeclareAsyncForEachEvent()
    {
        // Arrange
        var mockChannelProvider = new Mock<IChannelProvider>();
        var loggerMock = new Mock<ILogger<DeclareExchangeBackgroundService<ProductCreatedDomainEvent>>>();
        var service = new DeclareExchangeBackgroundService<ProductCreatedDomainEvent>(mockChannelProvider.Object, loggerMock.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        await service.StartAsync(cancellationToken);

        // Assert
        // Verify that ExchangeDeclareAsync was called for each DomainEvent found in TestAssembly
        mockChannelProvider.Verify(x => x.ExchangeDeclareAsync(typeof(ProductCreatedDomainEvent), It.IsAny<CancellationToken>()), Times.Once);
        mockChannelProvider.Verify(x => x.ExchangeDeclareAsync(typeof(UserCreatedDomainEvent), It.IsAny<CancellationToken>()), Times.Once);
        mockChannelProvider.Verify(x => x.ExchangeDeclareAsync(It.IsAny<Type>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_InternalException_WriteLogger()
    {
        // Arrange
        var mockChannelProvider = new Mock<IChannelProvider>();
        var loggerMock = new Mock<ILogger<DeclareExchangeBackgroundService<ProductCreatedDomainEvent>>>();

        mockChannelProvider.Setup(x => x.ExchangeDeclareAsync(It.IsAny<Type>(), It.IsAny<CancellationToken>())).Throws(new Exception("Internal exception"));

        var service = new DeclareExchangeBackgroundService<ProductCreatedDomainEvent>(mockChannelProvider.Object, loggerMock.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => service.StartAsync(cancellationToken));

        // Assert
        // Verify that ExchangeDeclareAsync was called for each DomainEvent found in TestAssembly
        loggerMock.VerifyLogging($"An error occurred while declaring the exchanges | Internal exception", LogLevel.Error, Times.Once());

        Assert.Equal("Internal exception", exception.Message);
    }
}
