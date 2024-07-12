using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;
using CodeDesignPlus.Net.RabbitMQ.Extensions;
using Moq;
using RabbitMQ.Client;
using O = Microsoft.Extensions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

public class RabbitPubSubServiceTest
{
    [Fact]
    public void Constructor_LoggerIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(null!, Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), Mock.Of<IRabbitConnection>(), Mock.Of<O.IOptions<CoreOptions>>()));
    }

    [Fact]
    public void Constructor_ServiceProviderIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), null!, Mock.Of<IDomainEventResolverService>(), Mock.Of<IRabbitConnection>(), Mock.Of<O.IOptions<CoreOptions>>()));
    }

    [Fact]
    public void Constructor_DomainEventResolverServiceIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), null!, Mock.Of<IRabbitConnection>(), Mock.Of<O.IOptions<CoreOptions>>()));
    }

    [Fact]
    public void Constructor_RabbitConnectionIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), null!, Mock.Of<O.IOptions<CoreOptions>>()));
    }

    [Fact]
    public void Constructor_CoreOptionsIsNull_ThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RabbitPubSubService(Mock.Of<ILogger<RabbitPubSubService>>(), Mock.Of<IServiceProvider>(), Mock.Of<IDomainEventResolverService>(), Mock.Of<IRabbitConnection>(), null!));
    }

    [Fact]
    public void Dispose_Called_ChannelAndRabbitConnectionDisposed()
    {
        // Arrange
        var channelMock = new Mock<IModel>();
        var rabbitConnectionMock = new Mock<IRabbitConnection>();
        var loggerMock = new Mock<ILogger<RabbitPubSubService>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var domainEventResolverServiceMock = new Mock<IDomainEventResolverService>();
        var coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        var connection = new Mock<IConnection>();

        channelMock.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());
        connection.Setup(c => c.CreateModel()).Returns(channelMock.Object);
        rabbitConnectionMock.SetupGet(r => r.Connection).Returns(connection.Object);

        var rabbitPubSubService = new RabbitPubSubService(loggerMock.Object, serviceProviderMock.Object, domainEventResolverServiceMock.Object, rabbitConnectionMock.Object, coreOptionsMock.Object);

        // Act
        rabbitPubSubService.Dispose();

        // Assert
        channelMock.Verify(c => c.Dispose(), Times.Once);
        rabbitConnectionMock.Verify(r => r.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_CalledTwice_ChannelAndRabbitConnectionDisposedOnce()
    {
        // Arrange
        var channelMock = new Mock<IModel>();
        var rabbitConnectionMock = new Mock<IRabbitConnection>();
        var loggerMock = new Mock<ILogger<RabbitPubSubService>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var domainEventResolverServiceMock = new Mock<IDomainEventResolverService>();
        var coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        var connection = new Mock<IConnection>();

        channelMock.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());
        connection.Setup(c => c.CreateModel()).Returns(channelMock.Object);
        rabbitConnectionMock.SetupGet(r => r.Connection).Returns(connection.Object);

        var rabbitPubSubService = new RabbitPubSubService(loggerMock.Object, serviceProviderMock.Object, domainEventResolverServiceMock.Object, rabbitConnectionMock.Object, coreOptionsMock.Object);

        // Act
        rabbitPubSubService.Dispose();
        rabbitPubSubService.Dispose();

        // Assert
        channelMock.Verify(c => c.Dispose(), Times.Once);
        rabbitConnectionMock.Verify(r => r.Dispose(), Times.Once);
    }


    [Fact]
    public void RabitPubSub_Destructor_DisposesConnection()
    {
        // Arrange
        var channelMock = new Mock<IModel>();
        var rabbitConnectionMock = new Mock<IRabbitConnection>();
        var loggerMock = new Mock<ILogger<RabbitPubSubService>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var domainEventResolverServiceMock = new Mock<IDomainEventResolverService>();
        var coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        var connection = new Mock<IConnection>();

        channelMock.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());
        connection.Setup(c => c.CreateModel()).Returns(channelMock.Object);
        rabbitConnectionMock.SetupGet(r => r.Connection).Returns(connection.Object);


        void action()
        {
            var rabbitPubSubService = new RabbitPubSubService(loggerMock.Object, serviceProviderMock.Object, domainEventResolverServiceMock.Object, rabbitConnectionMock.Object, coreOptionsMock.Object);
        }

        // Act
        action();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Assert
        Assert.True(true);
    }

}
