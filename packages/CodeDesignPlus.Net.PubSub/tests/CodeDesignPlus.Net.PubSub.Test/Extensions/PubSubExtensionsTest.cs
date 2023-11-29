using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.PubSub.Extensions;

namespace CodeDesignPlus.Net.PubSub.Test.Extensions;

public class PubSubExtensionsTest
{
    [Fact]
    public void IsAssignableGenericFrom_ValidEventHandler_ReturnsTrue()
    {
        // Act
        var result = typeof(UserRegisteredEventHandler).IsAssignableGenericFrom(typeof(IEventHandler<>));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAssignableGenericFrom_InvalidEventHandler_ReturnsFalse()
    {
        // Act
        var result = typeof(InvalidEventHandler).IsAssignableGenericFrom(typeof(IFake));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetEvents_EventsPresent_ReturnsListOfEvents()
    {
        // Act
        var events = PubSubExtensions.GetEvents();

        // Assert
        Assert.Contains(events, e => e == typeof(UserRegisteredEvent));
    }

    [Fact]
    public void GetEventHandlers_EventHandlersPresent_ReturnsListOfEventHandlers()
    {
        // Act
        var handlers = PubSubExtensions.GetEventHandlers();

        // Assert
        Assert.Contains(handlers, h => h == typeof(UserRegisteredEventHandler));
    }

    [Fact]
    public void GetInterfaceEventHandlerGeneric_ValidType_ReturnsExpectedInterface()
    {
        // Act
        var result = typeof(UserRegisteredEventHandler).GetInterfaceEventHandlerGeneric();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(IEventHandler<UserRegisteredEvent>), result);
    }

    [Fact]
    public void GetEventType_ValidInterface_ReturnsExpectedEventType()
    {
        // Act
        var result = typeof(IEventHandler<UserRegisteredEvent>).GetEventType();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(UserRegisteredEvent), result);
    }

    [Fact]
    public void GetPubSub_WhenCalled_ReturnsPubSubServiceType()
    {
        // Act
        var result = PubSubExtensions.GetPubSub();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(typeof(PubSubService), result);
    }
}
