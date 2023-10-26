using CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;

namespace CodeDesignPlus.Net.Event.Bus.Test.Services;

public class SubscriptionManagerTests
{
    private readonly Mock<ILogger<SubscriptionManager>> _loggerMock;
    private readonly SubscriptionManager _subscriptionManager;

    public SubscriptionManagerTests()
    {
        _loggerMock = new Mock<ILogger<SubscriptionManager>>();
        _subscriptionManager = new SubscriptionManager(_loggerMock.Object);
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SubscriptionManager(null));
    }

    [Fact]
    public void Constructor_ValidLogger_LogsInitialization()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        _ = new SubscriptionManager(loggerMock.Object);

        // Act & Assert
        loggerMock.VerifyLogging("SubscriptionManager initialized.", LogLevel.Information);

    }

    [Fact]
    public void GetEventKey_ValidEvent_ReturnsCorrectEventName()
    {
        // Arrange
        var manager = new SubscriptionManager(_loggerMock.Object);

        // Act
        var eventName = manager.GetEventKey<UserRegisteredEvent>();

        // Assert
        Assert.Equal("UserRegisteredEvent", eventName);
    }

    [Fact]
    public void AddSubscription_FirstTime_AddsEventAndHandler()
    {
        // Act
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        _loggerMock.Verify(log => log.LogInformation("Event {eventName} added to handlers.", "UserRegisteredEvent"), Times.Once);
        _loggerMock.Verify(log => log.LogInformation("EventHandler {TEventHandler} for event {eventName} registered.", "UserRegisteredEventHandler", "UserRegisteredEvent"), Times.Once);
    }

    [Fact]
    public void AddSubscription_AlreadyRegistered_ThrowsException()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act & Assert
        Assert.Throws<EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>>(() => _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>());
        _loggerMock.Verify(log => log.LogWarning("EventHandler {TEventHandler} for event {eventName} already registered.", "UserRegisteredEventHandler", "UserRegisteredEvent"), Times.Once);
    }

    [Fact]
    public void RemoveSubscription_ExistingHandler_RemovesHandler()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        _subscriptionManager.RemoveSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        _loggerMock.Verify(log => log.LogInformation("Removed subscription for EventHandler {TEventHandler} from event {eventName}.", "UserRegisteredEventHandler", "UserRegisteredEvent"), Times.Once);
        _loggerMock.Verify(log => log.LogInformation("Event {eventName} has no subscriptions and has been removed from handlers.", "UserRegisteredEvent"), Times.Once);
    }

    [Fact]
    public void RemoveSubscription_NonExistentHandler_LogsWarning()
    {
        // Act
        _subscriptionManager.RemoveSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        _loggerMock.Verify(log => log.LogWarning("Attempted to remove non-existent subscription for EventHandler {TEventHandler} from event {eventName}.", "UserRegisteredEventHandler", "UserRegisteredEvent"), Times.Once);
    }

    [Fact]
    public void HasSubscriptionsForEvent_WithRegisteredHandler_ReturnsTrue()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = _subscriptionManager.HasSubscriptionsForEvent<UserRegisteredEvent>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasSubscriptionsForEvent_WithNoRegisteredHandler_ReturnsFalse()
    {
        // Act
        var result = _subscriptionManager.HasSubscriptionsForEvent<UserRegisteredEvent>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHandlers_ForRegisteredEvent_ReturnsHandlers()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var handlers = _subscriptionManager.GetHandlers<UserRegisteredEvent>();

        // Assert
        Assert.Single(handlers);
        Assert.IsType<Subscription>(handlers.First());
    }

    [Fact]
    public void GetHandlers_ForUnregisteredEvent_ThrowsException()
    {
        // Assert
        Assert.Throws<EventIsNotRegisteredException>(() => _subscriptionManager.GetHandlers<UserRegisteredEvent>());
    }


    [Fact]
    public void FindSubscription_WithRegisteredHandler_ReturnsSubscription()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = _subscriptionManager.FindSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(UserRegisteredEventHandler), result.EventHandlerType);
    }

    [Fact]
    public void FindSubscription_WithNoRegisteredHandler_ReturnsNull()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = _subscriptionManager.FindSubscription<FakeEvent, FakeEventHandler>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindSubscription_ForUnregisteredEvent_ThrowsExceptionAndLogsWarning()
    {
        // Act & Assert
        var exception = Assert.Throws<EventIsNotRegisteredException>(() => _subscriptionManager.FindSubscription<UserRegisteredEvent, UserRegisteredEventHandler>());

        _loggerMock.VerifyLogging($"Attempted to find subscription for unregistered event {typeof(UserRegisteredEvent).Name}.", LogLevel.Warning);
    }

    [Fact]
    public void FindSubscriptions_WithRegisteredHandler_ReturnsSubscriptionsList()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = _subscriptionManager.FindSubscriptions<UserRegisteredEvent>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(typeof(UserRegisteredEventHandler), result.First().EventHandlerType);
    }

    [Fact]
    public void FindSubscriptions_ForUnregisteredEvent_ThrowsException()
    {
        // Act & Assert
        var exception = Assert.Throws<EventIsNotRegisteredException>(() => _subscriptionManager.FindSubscriptions<UserRegisteredEvent>());
    }

    [Fact]
    public void Clear_WithSubscriptions_ClearsAllSubscriptionsAndLogsInformation()
    {
        // Arrange
        _subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        _subscriptionManager.Clear();

        // Assert
        Assert.False(_subscriptionManager.HasSubscriptionsForEvent<UserRegisteredEvent>());
        _loggerMock.VerifyLogging("Subscription manager cleared all event handlers.", LogLevel.Information);
    }
}