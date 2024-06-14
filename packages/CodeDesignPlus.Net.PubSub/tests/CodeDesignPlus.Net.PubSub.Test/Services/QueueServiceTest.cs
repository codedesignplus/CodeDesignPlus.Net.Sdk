using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class QueueServiceTest
{
    private readonly Mock<UserRegisteredEventHandler> mockEventHandler;
    private readonly Mock<ILogger<QueueService<UserRegisteredEventHandler, UserRegisteredEvent>>> mockLogger;
    private readonly Mock<IOptions<PubSubOptions>> mockOptions;

    public QueueServiceTest()
    {
        mockEventHandler = new Mock<UserRegisteredEventHandler>();
        mockLogger = new Mock<ILogger<QueueService<UserRegisteredEventHandler, UserRegisteredEvent>>>();
        mockOptions = new Mock<IOptions<PubSubOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new PubSubOptions { SecondsWaitQueue = 1 });
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_When_EventHandlerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(null!, mockLogger.Object, mockOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_When_LoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, null!, mockOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_When_OptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, mockLogger.Object, null!));
    }

    [Fact]
    public void Constructor_LogsInitializationMessage()
    {
        var _ = new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, mockLogger.Object, mockOptions.Object);
        mockLogger.VerifyLogging("QueueService initialized.", LogLevel.Debug);
    }

    [Fact]
    public void Enqueue_ThrowsArgumentNullException_When_EventIsNull()
    {
        var service = new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, mockLogger.Object, mockOptions.Object);
        Assert.Throws<ArgumentNullException>(() => service.Enqueue(null!));
        mockLogger.VerifyLogging("Attempted to enqueue a null event.", LogLevel.Error);
    }

    [Fact]
    public void Enqueue_AddsEventToQueue_When_EventIsNew()
    {
        var service = new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, mockLogger.Object, mockOptions.Object);
        service.Enqueue(new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        });
        Assert.True(service.Any());
        mockLogger.VerifyLogging("Event of type UserRegisteredEvent enqueued.", LogLevel.Debug);
    }

    [Fact]
    public void Enqueue_SkipsEvent_When_EventAlreadyInQueue()
    {
        var userRegisteredEvent = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        };
        var service = new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, mockLogger.Object, mockOptions.Object);
        service.Enqueue(userRegisteredEvent);
        service.Enqueue(userRegisteredEvent);
        Assert.Equal(1, service.Count);
        mockLogger.VerifyLogging("Event of type UserRegisteredEvent was already in the queue. Skipping.", LogLevel.Warning);
    }

    [Fact]
    public async Task DequeueAsync_ProcessesEventsInQueue()
    {
        var userRegisteredEvent = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        };

        var handler = new UserRegisteredEventHandler();

        var service = new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(handler, mockLogger.Object, mockOptions.Object);
        service.Enqueue(userRegisteredEvent);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(2));  // Allow some time for processing before cancellation

        await service.DequeueAsync(cts.Token);

        mockLogger.VerifyLogging("Dequeueing event of type UserRegisteredEvent.", LogLevel.Debug);
        mockLogger.VerifyLogging("No events in the queue of type UserRegisteredEvent. Waiting...", LogLevel.Debug, Times.AtLeastOnce());
        mockLogger.VerifyLogging("DequeueAsync stopped due to cancellation token to type UserRegisteredEvent.", LogLevel.Debug);
    }


    [Fact]
    public async Task DequeueAsync_WhenHandleAsyncThrowsException_ErrorIsLogged()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();

        var loggerMock = new Mock<ILogger<QueueService<EventFailedHandler, EventFailed>>>();

        var @event = new EventFailed(Guid.NewGuid());
        var handler = new EventFailedHandler();

        var service = new QueueService<EventFailedHandler, EventFailed>(handler, loggerMock.Object, mockOptions.Object);

        service.Enqueue(@event);

        // Act
        var task = service.DequeueAsync(cancellationTokenSource.Token);
        await Task.Delay(1500); // Wait a bit for the method to process
        cancellationTokenSource.Cancel();
        await task;  // Ensure the method has fully completed

        // Assert
        loggerMock.VerifyLogging($"Error processing event of type {typeof(EventFailed).Name}.", LogLevel.Error);
    }
}