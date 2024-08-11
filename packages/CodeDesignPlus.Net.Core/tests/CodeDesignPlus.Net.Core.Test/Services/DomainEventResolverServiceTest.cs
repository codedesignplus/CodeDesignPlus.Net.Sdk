using CodeDesignPlus.Net.Core.Services;
using CodeDesignPlus.Net.Core.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.Core.Test.Services;

public class DomainEventResolverServiceTest
{
    private readonly IOptions<CoreOptions> options = Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions);

    [Fact]
    public void GetDomainEventType_WhenEventNameIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var service = new DomainEventResolverService(this.options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetDomainEventType(null));
    }

    [Fact]
    public void GetDomainEventType_WhenEventNameDoesNotExist_ThrowArgumentException()
    {
        // Arrange
        var service = new DomainEventResolverService(this.options);

        // Act & Assert
        var exception = Assert.Throws<CoreException>(() => service.GetDomainEventType("EventName"));

        Assert.Equal("The event type EventName does not exist", exception.Message);
    }

    [Fact]
    public void GetDomainEventType_WhenEventNameExist_ReturnType()
    {
        // Arrange
        var key = $"{ConfigurationUtil.CoreOptions.Business}.{ConfigurationUtil.CoreOptions.AppName}.v1.orderaggregate.created".ToLower();
        var service = new DomainEventResolverService(this.options);

        // Act
        var type = service.GetDomainEventType(key);

        // Assert
        Assert.NotNull(type);
    }

    [Fact]
    public void GetKeyEventGeneric_WhenTypeDoesNotHaveEventKeyAttribute_ThrowCoreException()
    {
        // Arrange
        var service = new DomainEventResolverService(this.options);

        // Act & Assert
        var exception = Assert.Throws<CoreException>(() => service.GetDomainEventType<DomainEvent>());

        Assert.Equal("The event DomainEvent does not have the KeyAttribute", exception.Message);
    }

    [Fact]
    public void GetKeyEventGeneric_WhenTypeHaveEventKeyAttribute_ReturnType()
    {
        // Arrange
        var service = new DomainEventResolverService(this.options);

        // Act
        var type = service.GetDomainEventType<OrderCreatedDomainEvent>();

        // Assert
        Assert.NotNull(type);
    }

    [Fact]
    public void GetKeyDomainEvent_WhenTypeExist_ReturnKey()
    {
        // Arrange
        var keyExpected = $"{ConfigurationUtil.CoreOptions.Business}.{ConfigurationUtil.CoreOptions.AppName}.v1.orderaggregate.created".ToLower();
        var service = new DomainEventResolverService(this.options);

        // Act
        var key = service.GetKeyDomainEvent<OrderCreatedDomainEvent>();

        // Assert
        Assert.NotNull(key);
        Assert.Equal(keyExpected, key);
    }

    [Fact]
    public void GetKeyEvent_WhenTypeIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var service = new DomainEventResolverService(this.options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.GetKeyDomainEvent(null));
    }

    [Fact]
    public void GetKeyEvent_WhenTypeDoesNotHaveEventKeyAttribute_ThrowCoreException()
    {
        // Arrange
        var service = new DomainEventResolverService(this.options);

        // Act & Assert
        var exception = Assert.Throws<CoreException>(() => service.GetKeyDomainEvent(typeof(DomainEvent)));

        Assert.Equal("The event DomainEvent does not have the KeyAttribute", exception.Message);
    }

    [Fact]
    public void GetKeyEvent_WhenTypeHaveEventKeyAttribute_ReturnType()
    {
        // Arrange
        var keyExpected = $"{ConfigurationUtil.CoreOptions.Business}.{ConfigurationUtil.CoreOptions.AppName}.v1.orderaggregate.created".ToLower();

        var service = new DomainEventResolverService(this.options);

        // Act
        var key = service.GetKeyDomainEvent(typeof(OrderCreatedDomainEvent));

        // Assert
        Assert.NotNull(key);
        Assert.Equal(keyExpected, key);
    }
}
