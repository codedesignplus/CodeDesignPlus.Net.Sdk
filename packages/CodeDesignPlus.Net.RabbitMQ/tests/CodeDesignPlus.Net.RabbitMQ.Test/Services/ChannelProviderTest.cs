using System;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using Moq;
using RabbitMQ.Client;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;


public class ChannelProviderTest
{
    private readonly Mock<IConnection> mockRabbitConnection = new();
    private readonly Mock<IRabbitConnection> mockConnection = new();
    private readonly Mock<IChannel> mockChannel = new();
    private readonly Mock<IDomainEventResolver> mockDomainEventResolver = new();
    private readonly IOptions<CoreOptions> coreOptions = Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions);

    [Fact]
    public async Task ExchangeDeclareAsync_NewDomainEvent_DeclaresExchangeAndReturnsName()
    {
        // Arrange
        mockDomainEventResolver
            .Setup(x => x.GetKeyDomainEvent(typeof(UserCreatedDomainEvent)))
            .Returns("order-created");

        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);

        // Act
        var exchangeName = await channelProvider.ExchangeDeclareAsync(typeof(UserCreatedDomainEvent));

        // Assert
        Assert.Equal("order-created", exchangeName);

        mockChannel.Verify(x => x.ExchangeDeclareAsync(
            "order-created",
            ExchangeType.Fanout,
            true,
            false,
            It.Is<IDictionary<string, object?>>(x => x.ContainsKey("x-cdp-bussiness") && x.ContainsKey("x-cdp-microservice")),
            false,
            false,
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task ExchangeDeclareAsync_NewDomainEventGeneric_DeclaresExchangeAndReturnsName()
    {
        // Arrange
        mockDomainEventResolver
          .Setup(x => x.GetKeyDomainEvent(typeof(UserCreatedDomainEvent)))
          .Returns("order-created");

        mockRabbitConnection
           .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);


        // Act
        var exchangeName = await channelProvider.ExchangeDeclareAsync(new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe"));

        // Assert
        Assert.Equal("order-created", exchangeName);

        mockChannel.Verify(x => x.ExchangeDeclareAsync(
           "order-created",
           ExchangeType.Fanout,
           true,
           false,
           It.Is<IDictionary<string, object?>>(x => x.ContainsKey("x-cdp-bussiness") && x.ContainsKey("x-cdp-microservice")),
           false,
           false,
           It.IsAny<CancellationToken>()
       ), Times.Once);
    }


    [Fact]
    public async Task ExchangeDeclareAsync_ExistingDomainEvent_ReturnsCachedExchangeName()
    {
        // Arrange
        mockDomainEventResolver
         .Setup(x => x.GetKeyDomainEvent(typeof(UserCreatedDomainEvent)))
         .Returns("order-created");

        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);


        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);
        await channelProvider.ExchangeDeclareAsync(typeof(UserCreatedDomainEvent));

        // Act
        var exchangeName = await channelProvider.ExchangeDeclareAsync(typeof(UserCreatedDomainEvent));

        // Assert
        Assert.Equal("order-created", exchangeName);
        mockChannel.Verify(x => x.ExchangeDeclareAsync(
           It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
           It.IsAny<bool>(),
           It.IsAny<IDictionary<string, object?>>(),
           It.IsAny<bool>(),
            It.IsAny<bool>(),
          It.IsAny<CancellationToken>()
      ), Times.Once);
    }

    [Fact]
    public async Task GetChannelPublishAsync_NewDomainEvent_ReturnsNewChannel()
    {
        // Arrange
        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);

        // Act
        var channel = await channelProvider.GetChannelPublishAsync(typeof(UserCreatedDomainEvent));

        // Assert
        Assert.NotNull(channel);
    }

    [Fact]
    public async Task GetChannelPublishAsync_NewDomainEventGeneric_ReturnsNewChannel()
    {
        // Arrange
        mockRabbitConnection
           .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);

        // Act
        var channel = await channelProvider.GetChannelPublishAsync(new UserCreatedDomainEvent(Guid.NewGuid(), "John Doe"));

        // Assert
        Assert.NotNull(channel);
    }

    [Fact]
    public async Task GetChannelPublishAsync_ExistingDomainEvent_ReturnsCachedChannel()
    {
        // Arrange
        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);
        await channelProvider.GetChannelPublishAsync(typeof(UserCreatedDomainEvent));

        // Act
        var channel = await channelProvider.GetChannelPublishAsync(typeof(UserCreatedDomainEvent));

        // Assert
        Assert.NotNull(channel);
        mockRabbitConnection.Verify(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetChannelConsumerAsync_NewEventHandler_ReturnsNewChannel()
    {
        // Arrange
        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);

        // Act
        var channel = await channelProvider.GetChannelConsumerAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>();

        // Assert
        Assert.NotNull(channel);
    }

    [Fact]
    public async Task GetChannelConsumerAsync_ExistingEventHandler_ReturnsCachedChannel()
    {
        // Arrange
        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);
        await channelProvider.GetChannelConsumerAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>();

        // Act
        var channel = await channelProvider.GetChannelConsumerAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>();

        // Assert
        Assert.NotNull(channel);
        mockRabbitConnection.Verify(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task SetConsumerTag_ExistingEventHandler_SetsConsumerTag()
    {
        // Arrange
        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);
        await channelProvider.GetChannelConsumerAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>();

        var consumerTag = "test-consumer";
        channelProvider.SetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(consumerTag);

        // Act
        var result = channelProvider.GetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>();

        // Assert
        Assert.Equal(consumerTag, result);
    }
    [Fact]
    public void GetConsumerTag_NonExistingEventHandler_ReturnsNull()
    {
        // Arrange
        mockRabbitConnection
            .Setup(x => x.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockConnection.SetupGet(x => x.Connection).Returns(mockRabbitConnection.Object);

        var channelProvider = new ChannelProvider(mockConnection.Object, mockDomainEventResolver.Object, coreOptions);

        // Act
        var result = channelProvider.GetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>();

        // Assert
        Assert.Null(result);
    }
}
