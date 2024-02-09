using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

public class SubscriptionManagerTests
{
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
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var manager = new SubscriptionManager(loggerMock.Object);

        // Act
        var eventName = manager.GetEventKey<UserRegisteredEvent>();

        // Assert
        Assert.Equal("user.registered.domain.event", eventName);
    }

    [Fact]
    public void AddSubscription_FirstTime_AddsEventAndHandler()
    {
        // Arrange        
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        // Act
        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        loggerMock.VerifyLogging("Event user.registered.domain.event added to handlers.", LogLevel.Information);
        loggerMock.VerifyLogging("EventHandler UserRegisteredEventHandler for event user.registered.domain.event registered.", LogLevel.Information);

    }

    [Fact]
    public void AddSubscription_AlreadyRegistered_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);
        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act & Assert
        Assert.Throws<EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>>(() => subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>());
        loggerMock.VerifyLogging("EventHandler UserRegisteredEventHandler for event user.registered.domain.event already registered.", LogLevel.Warning);
    }

    [Fact]
    public void RemoveSubscription_ExistingHandler_RemovesHandler()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);
        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        subscriptionManager.RemoveSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        loggerMock.VerifyLogging("Removed subscription for EventHandler UserRegisteredEventHandler from event user.registered.domain.event.", LogLevel.Information);
        loggerMock.VerifyLogging("Event user.registered.domain.event has no subscriptions and has been removed from handlers.", LogLevel.Information);
    }

    [Fact]
    public void HasSubscriptionsForEvent_WithRegisteredHandler_ReturnsTrue()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);
        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = subscriptionManager.HasSubscriptionsForEvent<UserRegisteredEvent>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasSubscriptionsForEvent_WithNoRegisteredHandler_ReturnsFalse()
    {
        // Act
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);
        var result = subscriptionManager.HasSubscriptionsForEvent<UserRegisteredEvent>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHandlers_ForRegisteredEvent_ReturnsHandlers()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);
        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var handlers = subscriptionManager.GetHandlers<UserRegisteredEvent>();

        // Assert
        Assert.Single(handlers);
        Assert.IsType<Subscription>(handlers.First());
    }

    [Fact]
    public void GetHandlers_ForUnregisteredEvent_ThrowsException()
    {
        // Arrange        
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        // Assert
        Assert.Throws<EventIsNotRegisteredException>(() => subscriptionManager.GetHandlers<UserRegisteredEvent>());
    }


    [Fact]
    public void FindSubscription_WithRegisteredHandler_ReturnsSubscription()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = subscriptionManager.FindSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(UserRegisteredEventHandler), result.EventHandlerType);
    }

    [Fact]
    public void FindSubscription_WithNoRegisteredHandler_ReturnsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        Assert.Throws<EventIsNotRegisteredException>(() => subscriptionManager.FindSubscription<FakeEvent, FakeEventHandler>());

        // Assert
        loggerMock.VerifyLogging($"Attempted to find subscription for unregistered event fake.event.domain.event.", LogLevel.Warning);
    }

    [Fact]
    public void FindSubscription_ForUnregisteredEvent_ThrowsExceptionAndLogsWarning()
    {
        // Arrange        
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<EventIsNotRegisteredException>(() => subscriptionManager.FindSubscription<UserRegisteredEvent, UserRegisteredEventHandler>());

        loggerMock.VerifyLogging($"Attempted to find subscription for unregistered event user.registered.domain.event.", LogLevel.Warning);
    }

    [Fact]
    public void FindSubscriptions_WithRegisteredHandler_ReturnsSubscriptionsList()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = subscriptionManager.FindSubscriptions<UserRegisteredEvent>();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(typeof(UserRegisteredEventHandler), result.First().EventHandlerType);
    }

    [Fact]
    public void FindSubscriptions_ForUnregisteredEvent_ThrowsException()
    {
        // Arrange         
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<EventIsNotRegisteredException>(() => subscriptionManager.FindSubscriptions<UserRegisteredEvent>());
    }

    [Fact]
    public void Clear_WithSubscriptions_ClearsAllSubscriptionsAndLogsInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        subscriptionManager.Clear();

        // Assert
        Assert.False(subscriptionManager.HasSubscriptionsForEvent<UserRegisteredEvent>());
        loggerMock.VerifyLogging("Subscription manager cleared all event handlers.", LogLevel.Information);
    }


    [Fact]
    public void Any_HandlersIsEmpty_ReturnsFalse()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        // Act
        var result = subscriptionManager.Any();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Any_HandlersHasAtLeastOneEvent_ReturnsTrue()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();

        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Act
        var result = subscriptionManager.Any();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RemoveSubscription_WhenLastSubscriptionIsRemoved_RaisesOnEventRemovedEvent()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SubscriptionManager>>();
        var subscriptionManager = new SubscriptionManager(loggerMock.Object);

        subscriptionManager.AddSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();
        var eventRaised = false;
        subscriptionManager.OnEventRemoved += (sender, args) =>
        {
            eventRaised = true;
            Assert.IsType<Subscription>(args);
        };

        // Act
        subscriptionManager.RemoveSubscription<UserRegisteredEvent, UserRegisteredEventHandler>();

        // Assert
        Assert.True(eventRaised, "OnEventRemoved event was not raised.");
    }
}