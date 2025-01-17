using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using Moq;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

public class InitializeBackgroundServiceTests
{

    [Fact]
    public async Task ExecuteAsync_WithDomainEvents_CallsExchangeDeclareAsyncForEachEvent()
    {
        // Arrange
        var mockChannelProvider = new Mock<IChannelProvider>();
        var service = new InitializeBackgroundService<ProductCreatedDomainEvent>(mockChannelProvider.Object);
        var cancellationToken = CancellationToken.None;

        // Act
        await service.StartAsync(cancellationToken);

        // Assert
        // Verify that ExchangeDeclareAsync was called for each DomainEvent found in TestAssembly
        mockChannelProvider.Verify(x => x.ExchangeDeclareAsync(typeof(ProductCreatedDomainEvent)), Times.Once);
        mockChannelProvider.Verify(x => x.ExchangeDeclareAsync(typeof(UserCreatedDomainEvent)), Times.Once);
        mockChannelProvider.Verify(x => x.ExchangeDeclareAsync(It.IsAny<Type>()), Times.Exactly(2));

    }
}
