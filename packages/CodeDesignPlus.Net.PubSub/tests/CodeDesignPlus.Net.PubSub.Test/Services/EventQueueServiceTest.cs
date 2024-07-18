using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;
using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class EventQueueServiceTest
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventQueueService(null, new Mock<IOptions<PubSubOptions>>().Object, new Mock<IMessage>().Object, new Mock<IActivityService>().Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventQueueService(new Mock<ILogger<EventQueueService>>().Object, null, new Mock<IMessage>().Object, new Mock<IActivityService>().Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMessageIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventQueueService(new Mock<ILogger<EventQueueService>>().Object, new Mock<IOptions<PubSubOptions>>().Object, null, new Mock<IActivityService>().Object));
    }

    [Fact]
    public void Constructor_Initialize_Successfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        // Act
        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, activityServiceMock.Object);

        // Assert
        Assert.NotNull(eventQueueService);
        loggerMock.VerifyLogging("EventQueueService initialized.", LogLevel.Debug);
    }

    [Fact]
    public async Task EnqueueAsync_WhenEventIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, activityServiceMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventQueueService.EnqueueAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task EnqueueAsync_WhenEventExistsInQueue_ShouldNotEnqueueEvent()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        var eventMock = new Mock<IDomainEvent>();

        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, activityServiceMock.Object);

        // Act
        await eventQueueService.EnqueueAsync(eventMock.Object, CancellationToken.None);
        await eventQueueService.EnqueueAsync(eventMock.Object, CancellationToken.None);

        // Assert
        loggerMock.VerifyLogging($"Event of type {eventMock.Object.GetType().Name} was already in the queue. Skipping.", LogLevel.Warning, Times.Once());
    }


    [Fact]
    public async Task EnqueueAsync_WhenActivityIsNull_ShouldEnqueueEvent()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();

        var @event = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "John Doe",
            User = "johndoe",
            Age = 30
        };

        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, null);

        var queue = eventQueueService.GetType().GetField("queue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(eventQueueService) as ConcurrentQueue<IDomainEvent>;

        // Act
        await eventQueueService.EnqueueAsync(@event, CancellationToken.None);

        // Assert
        loggerMock.VerifyLogging($"Event of type {@event.GetType().Name} enqueued.", LogLevel.Debug, Times.Once());

        Assert.Single(queue!);
        Assert.Contains(queue!, e => e.Equals(@event));
    }

    [Fact]
    public async Task EnqueueAsync_WhenEventIsNotNull_ShouldEnqueueEvent()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        var activity = new System.Diagnostics.Activity("EventQueueService.EnqueueAsync");

        activityServiceMock
            .Setup(a => a.StartActivity(It.IsAny<string>(), System.Diagnostics.ActivityKind.Internal, default))
            .Returns(activity);

        var @event = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "John Doe",
            User = "johndoe",
            Age = 30
        };

        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, activityServiceMock.Object);

        var queue = eventQueueService.GetType().GetField("queue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(eventQueueService) as ConcurrentQueue<IDomainEvent>;

        // Act
        await eventQueueService.EnqueueAsync(@event, CancellationToken.None);

        // Assert
        loggerMock.VerifyLogging($"Event of type {@event.GetType().Name} enqueued.", LogLevel.Debug, Times.Once());
        activityServiceMock.Verify(a => a.StartActivity("EventQueueService.EnqueueAsync", System.Diagnostics.ActivityKind.Internal, default), Times.Once());
        activityServiceMock.Verify(a => a.Inject(It.IsAny<System.Diagnostics.Activity>(), It.IsAny<IDomainEvent>()), Times.Once());

        Assert.Contains(activity.Tags, t => t.Key == "event.type" && t.Value == @event.GetType().Name);
        Assert.Contains(activity.Tags, t => t.Key == "event.id" && t.Value == @event.EventId.ToString());
        Assert.Contains(activity.Tags, t => t.Key == "event.aggregate_id" && t.Value == @event.AggregateId.ToString());

        Assert.Equal(System.Diagnostics.ActivityStatusCode.Ok, activity.Status);

        Assert.Single(queue!);
        Assert.Contains(queue!, e => e.Equals(@event));
    }

    [Fact]
    public async Task DequeueAsync_CancellationTokenIsCancelled_StopDequeue()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, activityServiceMock.Object);

        // Act
        await eventQueueService.DequeueAsync(new CancellationToken(true));

        // Assert
        loggerMock.VerifyLogging("DequeueAsync stopped due to cancellation token.", LogLevel.Warning, Times.Once());
    }

    [Fact]
    public void DequeueAsync_UnhandledException_WriteError()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var optionsMock = new Mock<IOptions<PubSubOptions>>();
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        messageMock
            .Setup(m => m.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Custom Exception"));

        var eventQueueService = new EventQueueService(loggerMock.Object, optionsMock.Object, messageMock.Object, activityServiceMock.Object);

        // Act
        _ = Task.Run(() => eventQueueService.DequeueAsync(cancellationTokenSource.Token));

        // Assert
        loggerMock.VerifyLogging("Error processing event.", LogLevel.Error, Times.AtLeastOnce());
        cancellationTokenSource.Cancel();
    }

    [Fact]
    public async Task DequeueAsync_QueueEmpty_WaitSeconds()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var options = Microsoft.Extensions.Options.Options.Create(Helpers.ConfigurationUtil.PubSubOptions);
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        var eventQueueService = new EventQueueService(loggerMock.Object, options, messageMock.Object, activityServiceMock.Object);

        // Act
        _ = Task.Run(() => eventQueueService.DequeueAsync(cancellationTokenSource.Token));

        // Wait for a few seconds to simulate waiting for events
        await Task.Delay(TimeSpan.FromSeconds(1));

        cancellationTokenSource.Cancel();

        await Task.Delay(TimeSpan.FromSeconds(1));

        // Assert
        loggerMock.VerifyLogging("No events in the queue. Waiting...", LogLevel.Debug, Times.AtLeastOnce());
        loggerMock.VerifyLogging("DequeueAsync stopped due to cancellation token.", LogLevel.Warning, Times.Once());
    }


    [Fact]
    public async Task DequeueAsync_ActivityServiceIsNull_ShouldPublishEvent()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var options = Microsoft.Extensions.Options.Options.Create(Helpers.ConfigurationUtil.PubSubOptions);
        var messageMock = new Mock<IMessage>();

        var @event = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "John Doe",
            User = "johndoe",
            Age = 30
        };

        var eventQueueService = new EventQueueService(loggerMock.Object, options, messageMock.Object, null);

        await eventQueueService.EnqueueAsync(@event, CancellationToken.None);

        // Act
        _ = Task.Run(() => eventQueueService.DequeueAsync(cancellationTokenSource.Token));

        // Wait for a few seconds to simulate waiting for events
        await Task.Delay(TimeSpan.FromSeconds(1));

        cancellationTokenSource.Cancel();

        await Task.Delay(TimeSpan.FromSeconds(1));

        // Assert
        loggerMock.VerifyLogging("Dequeueing event of type UserRegisteredEvent.", LogLevel.Debug, Times.Once());
        loggerMock.VerifyLogging("DequeueAsync stopped due to cancellation token.", LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task DequeueAsync_WhenQueueNotEmpty_ShouldPublishEvent()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var loggerMock = new Mock<ILogger<EventQueueService>>();
        var options = Microsoft.Extensions.Options.Options.Create(Helpers.ConfigurationUtil.PubSubOptions);
        var messageMock = new Mock<IMessage>();
        var activityServiceMock = new Mock<IActivityService>();

        var activity = new System.Diagnostics.Activity("EventQueueService.EnqueueAsync");
        var propagationContext = new OpenTelemetry.Context.Propagation.PropagationContext(activity.Context, new OpenTelemetry.Baggage());

        activityServiceMock
            .Setup(a => a.StartActivity(It.IsAny<string>(), System.Diagnostics.ActivityKind.Internal, propagationContext))
            .Returns(activity);

        activityServiceMock
            .Setup(a => a.Extract(It.IsAny<UserRegisteredEvent>()))
            .Returns(propagationContext);

        var @event = new UserRegisteredEvent(Guid.NewGuid())
        {
            Name = "John Doe",
            User = "johndoe",
            Age = 30
        };

        var eventQueueService = new EventQueueService(loggerMock.Object, options, messageMock.Object, activityServiceMock.Object);

        await eventQueueService.EnqueueAsync(@event, CancellationToken.None);

        // Act
        _ = Task.Run(() => eventQueueService.DequeueAsync(cancellationTokenSource.Token));

        // Wait for a few seconds to simulate waiting for events
        await Task.Delay(TimeSpan.FromSeconds(1));

        cancellationTokenSource.Cancel();

        await Task.Delay(TimeSpan.FromSeconds(1));

        // Assert
        activityServiceMock.Verify(a => a.Extract(It.IsAny<IDomainEvent>()), Times.Once());
        activityServiceMock.Verify(a => a.StartActivity("EventQueueService.DequeueAsync", System.Diagnostics.ActivityKind.Internal, propagationContext), Times.Once());

        Assert.Contains(activity.Tags, t => t.Key == "event.type" && t.Value == @event.GetType().Name);
        Assert.Contains(activity.Tags, t => t.Key == "event.id" && t.Value == @event.EventId.ToString());
        Assert.Contains(activity.Tags, t => t.Key == "event.aggregate_id" && t.Value == @event.AggregateId.ToString());
        Assert.Equal(System.Diagnostics.ActivityStatusCode.Ok, activity.Status);

        loggerMock.VerifyLogging("Dequeueing event of type UserRegisteredEvent.", LogLevel.Debug, Times.Once());
        loggerMock.VerifyLogging("DequeueAsync stopped due to cancellation token.", LogLevel.Warning, Times.Once());
    }

}
