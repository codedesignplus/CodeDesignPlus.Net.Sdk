using CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;

namespace CodeDesignPlus.Net.Event.Bus.Test.Services;

public class QueueServiceTest
{
    private readonly Mock<UserRegisteredEventHandler> mockEventHandler;
    private readonly Mock<ILogger<QueueService<UserRegisteredEventHandler, UserRegisteredEvent>>> mockLogger;
    private readonly Mock<IOptions<EventBusOptions>> mockOptions;

    public QueueServiceTest()
    {
        mockEventHandler = new Mock<UserRegisteredEventHandler>();
        mockLogger = new Mock<ILogger<QueueService<UserRegisteredEventHandler, UserRegisteredEvent>>>();
        mockOptions = new Mock<IOptions<EventBusOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new EventBusOptions { SecondsWaitQueue = 1 });
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
        mockLogger.VerifyLogging("QueueService initialized.", LogLevel.Information);
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
        service.Enqueue(new UserRegisteredEvent()
        {
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        });
        Assert.True(service.Any());
        mockLogger.VerifyLogging("Event of type UserRegisteredEvent enqueued.", LogLevel.Information);
    }

    [Fact]
    public void Enqueue_SkipsEvent_When_EventAlreadyInQueue()
    {
        var userRegisteredEvent = new UserRegisteredEvent()
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
        var userRegisteredEvent = new UserRegisteredEvent()
        {
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        };
        mockEventHandler.Setup(eh => eh.HandleAsync(userRegisteredEvent, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var service = new QueueService<UserRegisteredEventHandler, UserRegisteredEvent>(mockEventHandler.Object, mockLogger.Object, mockOptions.Object);
        service.Enqueue(userRegisteredEvent);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(2));  // Allow some time for processing before cancellation

        await service.DequeueAsync(cts.Token);

        mockLogger.VerifyLogging("Dequeueing event of type UserRegisteredEvent.", LogLevel.Information);
        mockLogger.VerifyLogging("No events in the queue of type UserRegisteredEvent. Waiting...", LogLevel.Debug);
        mockLogger.VerifyLogging("DequeueAsync stopped due to cancellation token to type UserRegisteredEvent.", LogLevel.Information);
    }
}