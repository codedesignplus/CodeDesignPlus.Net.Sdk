using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

public class RabbitPubSubServiceTest
{
    /// <summary>
    /// Queue name built by <see cref="Helpers.Events.UserCreatedDomainEventHandler"/> plus the core options of the test.
    /// </summary>
    private const string QueueName = "codedesignplus.test-rabbitmq.v1.notificationentity.notify_email_on_user_created";

    [Fact]
    public void Constructor_LoggerIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(null!, Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(), Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_ServiceProviderIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), null!, Mock.Of<IDomainEventResolver>(), Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
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
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(), null!, Mock.Of<O.IOptions<CoreOptions>>(), Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_CoreOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(), Mock.Of<IChannelProvider>(), null!, Mock.Of<O.IOptions<RabbitMQOptions>>()));
    }

    [Fact]
    public void Constructor_RabbitMQOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolver>(), Mock.Of<IChannelProvider>(), Mock.Of<O.IOptions<CoreOptions>>(), null!));
    }

    [Fact]
    public void UnsubscribeAsync_ConsumerTagIsNull_ReturnTaskCompleted()
    {
        // Arrange
        var channelMock = new Mock<IChannel>();
        var channelProviderMock = new Mock<IChannelProvider>();
        var loggerMock = new Mock<ILogger<RabbitPubSubService>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var domainEventResolverServiceMock = new Mock<IDomainEventResolver>();
        var coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        var connection = new Mock<IConnection>();
        var rabbitMQOptions = new Mock<IOptions<RabbitMQOptions>>();

        rabbitMQOptions.SetupGet(r => r.Value).Returns(new RabbitMQOptions());

        // channelMock.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());
        // connection.Setup(c => c.CreateModel()).Returns(channelMock.Object);
        channelProviderMock.Setup(x => x.GetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>()).Returns((string)null!);

        var rabbitPubSubService = new RabbitPubSubService(loggerMock.Object, serviceProviderMock.Object, domainEventResolverServiceMock.Object, channelProviderMock.Object, coreOptionsMock.Object, rabbitMQOptions.Object);

        // Act
        var result = rabbitPubSubService.UnsubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Assert
        Assert.True(result.IsCompletedSuccessfully);

        channelProviderMock.Verify(x => x.GetConsumerTag<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(), Times.Once);
    }

    [Fact]
    public async Task SubscribeAsync_DefaultOptions_DeclareQuorumQueueWithDeliveryLimit()
    {
        // Arrange
        var options = new RabbitMQOptions { MaxRetry = 3 };
        var arguments = new Dictionary<string, IDictionary<string, object>>();

        var service = BuildService(options, arguments, out _);

        // Act
        await service.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Assert
        var queue = arguments[QueueName];

        // Sin x-queue-type quorum el broker no cuenta las entregas y sin x-delivery-limit no corta el bucle:
        // un error de infraestructura reencolaba el mismo mensaje para siempre.
        Assert.Equal("quorum", queue["x-queue-type"]);

        // Una entrega por encima de MaxRetry: decide primero el consumidor, el broker solo es la red de seguridad.
        Assert.Equal(options.MaxRetry + 1, queue["x-delivery-limit"]);
    }

    [Fact]
    public async Task SubscribeAsync_DeliveryLimitConfigured_RespectConfiguredValue()
    {
        // Arrange
        var options = new RabbitMQOptions { MaxRetry = 3 };
        options.QueueArguments.DeliveryLimit = 7;

        var arguments = new Dictionary<string, IDictionary<string, object>>();

        var service = BuildService(options, arguments, out _);

        // Act
        await service.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Assert
        Assert.Equal(7, arguments[QueueName]["x-delivery-limit"]);
    }

    [Fact]
    public async Task SubscribeAsync_ClassicQueue_DoesNotDeclareDeliveryLimit()
    {
        // Arrange
        var options = new RabbitMQOptions { MaxRetry = 3 };
        options.QueueArguments.QueueType = "classic";

        var arguments = new Dictionary<string, IDictionary<string, object>>();

        var service = BuildService(options, arguments, out _);

        // Act
        await service.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Assert
        // x-delivery-limit solo existe en colas quorum: declararlo en una clasica cierra el canal.
        Assert.False(arguments[QueueName].ContainsKey("x-delivery-limit"));
    }

    [Fact]
    public async Task SubscribeAsync_DlqQueue_InheritsQueueTypeWithoutTtl()
    {
        // Arrange
        var options = new RabbitMQOptions { MaxRetry = 3 };
        var arguments = new Dictionary<string, IDictionary<string, object>>();

        var service = BuildService(options, arguments, out _);

        // Act
        await service.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Assert
        var dlq = arguments[$"{QueueName}.dlx"];

        Assert.Equal("quorum", dlq["x-queue-type"]);

        // La DLQ no lleva TTL ni limite de entregas: borraria los mensajes muertos antes de que nadie los revise.
        Assert.False(dlq.ContainsKey("x-message-ttl"));
        Assert.False(dlq.ContainsKey("x-delivery-limit"));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(9, true)]
    public async Task RecivedEvent_InfrastructureError_RequeueUntilDeliveryCountReachesMaxRetry(int deliveryCount, bool expectedToDlq)
    {
        // Arrange
        var options = new RabbitMQOptions { MaxRetry = 3 };
        var service = BuildService(options, [], out var channelMock);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?> { { "x-delivery-count", (long)deliveryCount } }
        };

        // El proveedor de servicios vacio hace fallar la creacion del scope: es un error de infraestructura,
        // no un CodeDesignPlusException, que es justo el caso que se reencolaba sin fin.
        var eventArguments = new BasicDeliverEventArgs("tag", 1, false, "exchange", string.Empty, properties, ReadOnlyMemory<byte>.Empty);

        // Act
        await service.RecivedEvent<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(channelMock.Object, eventArguments, CancellationToken.None);

        // Assert
        channelMock.Verify(x => x.BasicNackAsync(1, false, !expectedToDlq, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RecivedEvent_DeliveryCountAsInt_IsRead()
    {
        // Arrange
        var options = new RabbitMQOptions { MaxRetry = 3 };
        var service = BuildService(options, [], out var channelMock);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?> { { "x-delivery-count", 3 } }
        };

        var eventArguments = new BasicDeliverEventArgs("tag", 1, false, "exchange", string.Empty, properties, ReadOnlyMemory<byte>.Empty);

        // Act
        await service.RecivedEvent<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(channelMock.Object, eventArguments, CancellationToken.None);

        // Assert
        channelMock.Verify(x => x.BasicNackAsync(1, false, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Builds the service with mocked collaborators and captures the arguments of every declared queue.
    /// </summary>
    private static RabbitPubSubService BuildService(RabbitMQOptions options, Dictionary<string, IDictionary<string, object>> declaredQueues, out Mock<IChannel> channelMock)
    {
        channelMock = new Mock<IChannel>();

        var capture = declaredQueues;

        channelMock
            .Setup(x => x.QueueDeclareAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback((string queue, bool _, bool _, bool _, IDictionary<string, object?> arguments, bool _, bool _, CancellationToken _) =>
            {
                capture[queue] = arguments?.ToDictionary(x => x.Key, x => x.Value!) ?? [];
            })
            .ReturnsAsync(new QueueDeclareOk("queue", 0, 0));

        var channelProviderMock = new Mock<IChannelProvider>();
        channelProviderMock
            .Setup(x => x.GetChannelConsumerAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(channelMock.Object);

        var domainEventResolverMock = new Mock<IDomainEventResolver>();
        domainEventResolverMock.Setup(x => x.GetKeyDomainEvent<UserCreatedDomainEvent>()).Returns("user.created");

        var coreOptionsMock = new Mock<O.IOptions<CoreOptions>>();
        coreOptionsMock.SetupGet(x => x.Value).Returns(Helpers.ConfigurationUtil.CoreOptions);

        var rabbitMQOptionsMock = new Mock<O.IOptions<RabbitMQOptions>>();
        rabbitMQOptionsMock.SetupGet(x => x.Value).Returns(options);

        return new RabbitPubSubService(
            Mock.Of<ILogger<RabbitPubSubService>>(),
            Mock.Of<IServiceProvider>(),
            domainEventResolverMock.Object,
            channelProviderMock.Object,
            coreOptionsMock.Object,
            rabbitMQOptionsMock.Object);
    }
}
