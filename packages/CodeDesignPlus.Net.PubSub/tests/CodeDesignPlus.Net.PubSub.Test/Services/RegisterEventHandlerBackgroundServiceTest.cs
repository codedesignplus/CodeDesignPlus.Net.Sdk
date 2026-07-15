using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class RegisterEventHandlerBackgroundServiceTest
{
    private readonly Mock<ILogger<RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>> mockLogger;
    private readonly Mock<IMessage> mockMessage;
    private readonly Mock<ISubscriptionTracker> mockTracker;

    public RegisterEventHandlerBackgroundServiceTest()
    {
        mockLogger = new Mock<ILogger<RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>>();
        mockMessage = new Mock<IMessage>();
        mockTracker = new Mock<ISubscriptionTracker>();
    }

    [Fact]
    public async Task ExecuteAsync_SubscribesSuccessfully_MarksTrackerReady()
    {
        // Arrange
        mockMessage
            .Setup(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(
            mockMessage.Object, mockTracker.Object, mockLogger.Object);

        // Act
        await service.StartAsync(CancellationToken.None);
        await Task.Delay(500);

        // Assert
        mockMessage.Verify(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()), Times.Once);
        mockTracker.Verify(x => x.MarkSubscribed(typeof(UserRegisteredEventHandler).Name), Times.Once);
        mockTracker.Verify(x => x.MarkFailed(It.IsAny<string>()), Times.Never);

        mockLogger.VerifyLogging(
            $"Starting subscription of {typeof(UserRegisteredEventHandler).Name} for event type {typeof(UserRegisteredEvent).Name}.",
            LogLevel.Information);

        mockLogger.VerifyLogging(
            $"Successfully subscribed {typeof(UserRegisteredEventHandler).Name} for event type {typeof(UserRegisteredEvent).Name}.",
            LogLevel.Information);
    }

    [Fact]
    public async Task ExecuteAsync_FailsAllRetries_MarksTrackerFailed()
    {
        // Arrange
        mockMessage
            .Setup(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Connection refused"));

        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(
            mockMessage.Object, mockTracker.Object, mockLogger.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(TimeSpan.FromSeconds(25), cts.Token).ContinueWith(_ => { });

        // Assert
        mockMessage.Verify(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()), Times.Exactly(10));
        mockTracker.Verify(x => x.MarkFailed(typeof(UserRegisteredEventHandler).Name), Times.Once);
        mockTracker.Verify(x => x.MarkSubscribed(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_FailsThenSucceeds_MarksTrackerReady()
    {
        // Arrange
        var callCount = 0;
        mockMessage
            .Setup(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                callCount++;
                if (callCount < 3)
                    throw new Exception("Connection refused");
                return Task.CompletedTask;
            });

        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(
            mockMessage.Object, mockTracker.Object, mockLogger.Object);

        // Act
        await service.StartAsync(CancellationToken.None);
        await Task.Delay(TimeSpan.FromSeconds(10));

        // Assert
        mockMessage.Verify(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()), Times.Exactly(3));
        mockTracker.Verify(x => x.MarkSubscribed(typeof(UserRegisteredEventHandler).Name), Times.Once);
        mockTracker.Verify(x => x.MarkFailed(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_StopsGracefully()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        mockMessage
            .Setup(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Connection refused"));

        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(
            mockMessage.Object, mockTracker.Object, mockLogger.Object);

        // Act
        await service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await Task.Delay(500);

        // Assert
        mockTracker.Verify(x => x.MarkFailed(It.IsAny<string>()), Times.Never);
    }
}
