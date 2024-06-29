using CodeDesignPlus.Net.Core.Services;
using CodeDesignPlus.Net.Core.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.Core.Test.Services;

public class DomainEventResolverServiceTest
{

    [Fact]
    public void GetDomainEventType_WhenEventNameIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetDomainEventType(null));
    }

    [Fact]
    public void GetDomainEventType_WhenEventNameDoesNotExist_ThrowArgumentException()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act & Assert
        var exception = Assert.Throws<CoreException>(() => service.GetDomainEventType("EventName"));

        Assert.Equal("The event type EventName does not exist", exception.Message);
    }

    [Fact]
    public void GetDomainEventType_WhenEventNameExist_ReturnType()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act
        var type = service.GetDomainEventType("order.created");

        // Assert
        Assert.NotNull(type);
    }

    [Fact]
    public void GetKeyEventGeneric_WhenTypeDoesNotHaveEventKeyAttribute_ThrowCoreException()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act & Assert
        var exception = Assert.Throws<CoreException>(() => service.GetDomainEventType<DomainEvent>());

        Assert.Equal("The event DomainEvent does not have the KeyAttribute", exception.Message);
    }

    [Fact]
    public void GetKeyEventGeneric_WhenTypeHaveEventKeyAttribute_ReturnType()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act
        var type = service.GetDomainEventType<OrderCreatedDomainEvent>();

        // Assert
        Assert.NotNull(type);
    }

    [Fact]
    public void GetKeyDomainEvent_WhenTypeExist_ReturnKey()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act
        var key = service.GetKeyDomainEvent<OrderCreatedDomainEvent>();

        // Assert
        Assert.NotNull(key);
    }

    [Fact]
    public void GetKeyEvent_WhenTypeIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetKeyDomainEvent(null));
    }

    [Fact]
    public void GetKeyEvent_WhenTypeDoesNotHaveEventKeyAttribute_ThrowCoreException()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act & Assert
        var exception = Assert.Throws<CoreException>(() => service.GetKeyDomainEvent(typeof(DomainEvent)));

        Assert.Equal("The event DomainEvent does not have the KeyAttribute", exception.Message);
    }

    [Fact]
    public void GetKeyEvent_WhenTypeHaveEventKeyAttribute_ReturnType()
    {
        // Arrange
        var service = new DomainEventResolverService();

        // Act
        var key = service.GetKeyDomainEvent(typeof(OrderCreatedDomainEvent));

        // Assert
        Assert.NotNull(key);
    }
}
