using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class QueueBackgroundServiceTest
{
    private readonly Mock<IQueueService<UserRegisteredEventHandler, UserRegisteredEvent>> mockQueueService;
    private readonly Mock<ILogger<QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>> mockLogger;

    public QueueBackgroundServiceTest()
    {
        mockQueueService = new Mock<IQueueService<UserRegisteredEventHandler, UserRegisteredEvent>>();
        mockLogger = new Mock<ILogger<QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_When_QueueServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(null!, mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_When_LoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockQueueService.Object, null!));
    }

    [Fact]
    public void Constructor_LogsInitializationMessage()
    {
        var _ = new QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockQueueService.Object, mockLogger.Object);

        mockLogger.VerifyLogging("QueueBackgroundService for EventHandler: UserRegisteredEventHandler and Event: UserRegisteredEvent has been initialized.", LogLevel.Information);
    }

    [Fact]
    public async Task ExecuteAsync_CallsDequeueAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var service = new QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockQueueService.Object, mockLogger.Object);

        await service.StartAsync(tokenSource.Token);

        mockQueueService.Verify(q => q.DequeueAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_LogsOnStart()
    {
        var service = new QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockQueueService.Object, mockLogger.Object);

        await service.StartAsync(CancellationToken.None);

        mockLogger.VerifyLogging("Background service for event handling started.", LogLevel.Information);
    }

    [Fact]
    public async Task ExecuteAsync_LogsOnCancellation()
    {
        var cts = new CancellationTokenSource();
        var service = new QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockQueueService.Object, mockLogger.Object);

        cts.Cancel();

        await service.StartAsync(cts.Token);

        mockLogger.VerifyLogging("Background service for event handling is stopping.", LogLevel.Information);
    }
}
