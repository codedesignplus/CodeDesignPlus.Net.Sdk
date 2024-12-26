using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class EventQueueBackgroundServiceTest
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenQueueServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventQueueBackgroundService(null, Mock.Of<ILogger<EventQueueBackgroundService>>()));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventQueueBackgroundService(Mock.Of<IEventQueueService>(), null));
    }

    [Fact]
    public void Constructor_Initialize_Successfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueBackgroundService>>();
        var eventQueueServiceMock = new Mock<IEventQueueService>();

        // Act
        var eventQueueBackgroundService = new EventQueueBackgroundService(eventQueueServiceMock.Object, loggerMock.Object);

        // Assert
        Assert.NotNull(eventQueueBackgroundService);
        loggerMock.VerifyLogging("EventQueueBackgroundService has been initialized.", LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_DequeueAsync_Called()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueBackgroundService>>();
        var eventQueueServiceMock = new Mock<IEventQueueService>();
        var cancellationTokenSource = new CancellationTokenSource();

        var eventQueueBackgroundService = new EventQueueBackgroundService(eventQueueServiceMock.Object, loggerMock.Object);

        // Act
        await eventQueueBackgroundService.StartAsync(cancellationTokenSource.Token);

        await Task.Delay(2000);

        cancellationTokenSource.Cancel();

        // Assert
        eventQueueServiceMock.Verify(e => e.DequeueAsync(It.IsAny<CancellationToken>()), Times.Once);
        loggerMock.VerifyLogging("EventQueueBackgroundService service for event handling started.", LogLevel.Information, Times.Once());
        loggerMock.VerifyLogging("EventQueueBackgroundService service for event handling is stopping.", LogLevel.Information, Times.Once());
    }
}
