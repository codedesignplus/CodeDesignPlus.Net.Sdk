using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using Moq;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class PubSubBackgroundServiceTest
{
    private readonly Mock<ILogger<EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>> _mockLogger;
    private readonly Mock<IMessage> _mockPubSub;

    public PubSubBackgroundServiceTest()
    {
        _mockLogger = new Mock<ILogger<EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>>>();
        _mockPubSub = new Mock<IMessage>();
    }


    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenPubSubIsNull()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            var service = new EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>( null, _mockLogger.Object);
        });
    }

    //[Fact]
    //public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    //{
    //    // Assert
    //    Assert.Throws<ArgumentNullException>(() =>
    //    {
    //        // Act
    //        var service = new EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>(_mockPubSub.Object, null);
    //    });
    //}

    //[Fact]
    //public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
    //{
    //    // Act
    //    var service = new EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>( _mockPubSub.Object, _mockLogger.Object);

    //    // Assert 
    //    Assert.NotNull(service);
    //    _mockLogger.VerifyLogging($"EventHandlerBackgroundService for EventHandler: {typeof(UserRegisteredEventHandler).Name} and Event: {typeof(UserRegisteredEvent).Name} has been initialized.", LogLevel.Information);
    //}

    //[Fact]
    //public async Task ExecuteAsync_RegistersSubscriptionAndStartsListening()
    //{
    //    // Arrange
    //    var service = new EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>( _mockPubSub.Object, _mockLogger.Object);

    //    // Act
    //    await service.StartAsync(CancellationToken.None);

    //    // Assert
    //    _mockPubSub.Verify(eb => eb.SubscribeAsync<UserRegisteredEvent, UserRegisteredEventHandler>(It.IsAny<CancellationToken>()), Times.Once);

    //    // Additional: Check if logs are written. This is an example for one of the log messages:
    //    _mockLogger.VerifyLogging($"Starting execution of {typeof(UserRegisteredEventHandler).Name} for event type {typeof(UserRegisteredEvent).Name}.", LogLevel.Information);
    //}
}