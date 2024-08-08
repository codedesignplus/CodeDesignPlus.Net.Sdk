using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using Moq;
using RabbitMQ.Client;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

public class RabbitPubSubServiceTest
{
    [Fact]
    public void Constructor_LoggerIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(null!, Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_ServiceProviderIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), null!, Mock.Of<IDomainEventResolverService>(), Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_DomainEventResolverServiceIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), null!, Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_RabbitConnectionIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), null!, Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_CoreOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), Mock.Of<IChannelProvider>(), null!, Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_RabbitMQOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), null!));
    }

    [Fact]
    public void UnsubscribeAsync_ConsumerTagIsNull_ReturnTaskCompleted()
    {
        // Arrange
        var channelMock = new Mock<IModel>();
        var channelProviderMock = new Mock<IChannelProvider>();
        var loggerMock = new Mock<ILogger<RabbitPubSubService>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var domainEventResolverServiceMock = new Mock<IDomainEventResolverService>();
        var coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        var connection = new Mock<IConnection>();
        var rabbitMQOptions = new Mock<IOptions<RabbitMQOptions>>();

        rabbitMQOptions.SetupGet(r => r.Value).Returns(new RabbitMQOptions());

        channelMock.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());
        connection.Setup(c => c.CreateModel()).Returns(channelMock.Object);
        channelProviderMock.Setup(x => x.GetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>()).Returns((string)null!);

        var rabbitPubSubService = new RabbitPubSubService(loggerMock.Object, serviceProviderMock.Object, domainEventResolverServiceMock.Object, channelProviderMock.Object, coreOptionsMock.Object, rabbitMQOptions.Object);

        // Act
        var result = rabbitPubSubService.UnsubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Assert
        Assert.True(result.IsCompletedSuccessfully);
        channelProviderMock.Verify(x => x.GetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(), Times.Once);
    }

}
