using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class RegisterEventHandlerBackgroundServiceTest
{
    private readonly Mock<ILogger<RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>> mockLogger;
    private readonly Mock<IMessage> mockPubSub;

    public RegisterEventHandlerBackgroundServiceTest()
    {
        mockLogger = new Mock<ILogger<RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>>();
        mockPubSub = new Mock<IMessage>();
    }


    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenPubSubIsNull()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(null, mockLogger.Object);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockPubSub.Object, null);
        });
    }

    [Fact]
    public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
    {
        // Act
        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockPubSub.Object, mockLogger.Object);

        // Assert 
        Assert.NotNull(service);
        mockLogger.VerifyLogging($"RegisterEventHandlerBackgroundService for EventHandler: {typeof(UserRegisteredEventHandler).Name} and Event: {typeof(UserRegisteredEvent).Name} has been initialized.", LogLevel.Information);
    }

    [Fact]
    public async Task ExecuteAsync_RegistersSubscriptionAndStartsListening()
    {
        // Arrange
        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockPubSub.Object, mockLogger.Object);

        // Act
        await service.StartAsync(CancellationToken.None);

        await Task.Delay(1000);

        // Assert
        mockPubSub.Verify(eb => eb.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()), Times.Once);

        // Additional: Check if logs are written. This is an example for one of the log messages:
        mockLogger.VerifyLogging($"Starting execution of {typeof(UserRegisteredEventHandler).Name} for event type {typeof(UserRegisteredEvent).Name}.", LogLevel.Information);
    }

    [Fact]
    public async Task ExecuteAsync_InternalException_WriteLogger()
    {
        // Arrange
        mockPubSub
            .Setup(x => x.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()))
            .Throws(new Exception("An error occurred while registering the event handler."));

        var service = new RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(mockPubSub.Object, mockLogger.Object);

        // Act
        await service.StartAsync(CancellationToken.None);

        await Task.Delay(1000);

        // Assert
        mockPubSub.Verify(eb => eb.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()), Times.Once);

        // Additional: Check if logs are written. This is an example for one of the log messages:
        mockLogger.VerifyLogging($"An error occurred while registering the event handler {typeof(UserRegisteredEventHandler).Name} for event type {typeof(UserRegisteredEvent).Name}.", LogLevel.Error);
    }
}