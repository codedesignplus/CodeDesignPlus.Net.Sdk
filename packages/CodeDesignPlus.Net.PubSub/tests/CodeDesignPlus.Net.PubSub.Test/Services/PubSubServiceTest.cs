using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class PubSubTest
{
    private readonly Mock<IMessage> messageMock;
    private readonly Mock<IOptions<PubSubOptions>> optionsMock;
    private readonly Mock<IServiceProvider> serviceProviderMock;
    private readonly Mock<ILogger<PubSubService>> loggerMock;

    public PubSubTest()
    {
        this.messageMock = new Mock<IMessage>();
        this.optionsMock = new Mock<IOptions<PubSubOptions>>();
        this.serviceProviderMock = new Mock<IServiceProvider>();
        this.loggerMock = new Mock<ILogger<PubSubService>>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMessageIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(null, this.optionsMock.Object, this.serviceProviderMock.Object, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, null, this.serviceProviderMock.Object, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceProviderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, this.optionsMock.Object, null, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, this.optionsMock.Object, this.serviceProviderMock.Object, null));
    }

    [Fact]
    public void Constructor_Initialize_Successfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PubSubService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var pubSub = new PubSubService(messageMock.Object, optionsMock.Object, serviceProviderMock.Object, loggerMock.Object);

        // Assert
        Assert.NotNull(pubSub);
        loggerMock.VerifyLogging("PubSubService initialized.", LogLevel.Debug);
    }

    [Fact]
    public async Task PublishAsync_UseQueueTrue_EnqueueAsyncCalled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new PubSubOptions { UseQueue = true });
        var eventMock = new Mock<IDomainEvent>();
        var eventQueueServiceMock = new Mock<IEventQueue>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(eventQueueServiceMock.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var pubSub = new PubSubService(this.messageMock.Object, options, serviceProvider, this.loggerMock.Object);

        // Act
        await pubSub.PublishAsync(eventMock.Object, CancellationToken.None);

        // Assert
        eventQueueServiceMock.Verify(e => e.EnqueueAsync(eventMock.Object, CancellationToken.None), Times.Once);
        this.messageMock.Verify(m => m.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        loggerMock.VerifyLogging($"UseQueue is true, enqueuing event of type {eventMock.Object.GetType().Name}.", LogLevel.Debug);
    }

    [Fact]
    public async Task PublishAsync_UseQueueFalse_PublishAsyncCalled()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new PubSubOptions { UseQueue = false });
        var eventMock = new Mock<IDomainEvent>();
        var pubSub = new PubSubService(this.messageMock.Object, options, this.serviceProviderMock.Object, this.loggerMock.Object);

        // Act
        await pubSub.PublishAsync(eventMock.Object, CancellationToken.None);

        // Assert
        this.messageMock.Verify(m => m.PublishAsync(eventMock.Object, CancellationToken.None), Times.Once);
        loggerMock.VerifyLogging($"UseQueue is false, publishing event of type {eventMock.Object.GetType().Name}.", LogLevel.Debug);
    }

    [Fact]
    public async Task PublishAsync_MultipleEvents_PublishAsyncCalledForEachEvent()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new PubSubOptions { UseQueue = false });
        var eventMocks = new List<Mock<IDomainEvent>>
        {
            new(),
            new(),
            new()
        };
        var pubSub = new PubSubService(this.messageMock.Object, options, this.serviceProviderMock.Object, this.loggerMock.Object);

        // Act
        await pubSub.PublishAsync(eventMocks.Select(e => e.Object).ToList(), CancellationToken.None);

        // Assert
        foreach (var eventMock in eventMocks)
        {
            this.messageMock.Verify(m => m.PublishAsync(eventMock.Object, CancellationToken.None), Times.Once);
        }
    }
}
