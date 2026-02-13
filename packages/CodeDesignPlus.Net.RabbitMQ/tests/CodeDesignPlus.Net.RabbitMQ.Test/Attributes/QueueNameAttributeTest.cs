using CodeDesignPlus.Net.RabbitMQ.Attributes;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Attributes;

public class QueueNameAttributeTest
{
    [Fact]
    public void Constructor_WithEntityAndAction_SetsProperties()
    {
        // Arrange
        var entity = "Order";
        var action = "Created";

        // Act
        var attr = new QueueNameAttribute(entity, action);

        // Assert
        Assert.Equal(entity, attr.Entity);
        Assert.Equal(action, attr.Action);
        Assert.Null(attr.Business);
    }

    [Fact]
    public void Constructor_WithBusinessEntityAndAction_SetsProperties()
    {
        // Arrange
        var business = "Sales";
        var entity = "Order";
        var action = "Created";

        // Act
        var attr = new QueueNameAttribute(business, entity, action);

        // Assert
        Assert.Equal(business, attr.Business);
        Assert.Equal(entity, attr.Entity);
        Assert.Equal(action, attr.Action);
    }

    [Fact]
    public void GetQueueName_ReturnsExpectedFormat()
    {
        // Arrange
        var attr = new QueueNameAttribute("Sales", "Order", "Created");
        var appName = "MyApp";
        var business = "DefaultBusiness";
        var version = "v1";

        // Act
        var queueName = attr.GetQueueName(appName, business, version);

        // Assert
        Assert.Equal("sales.myapp.v1.order.created", queueName);
    }

    [Fact]
    public void GetQueueName_UsesProvidedBusinessIfNotSet()
    {
        // Arrange
        var attr = new QueueNameAttribute("Order", "Created");
        var appName = "MyApp";
        var business = "DefaultBusiness";
        var version = "v1";

        // Act
        var queueName = attr.GetQueueName(appName, business, version);

        // Assert
        Assert.Equal("defaultbusiness.myapp.v1.order.created", queueName);
    }

    [Fact]
    public void GenericAttribute_SetsEntityFromType()
    {
        // Act
        var attr = new QueueNameAttribute<DummyEntity>("Created");

        // Assert
        Assert.Equal(nameof(DummyEntity), attr.Entity);
        Assert.Equal("Created", attr.Action);
        Assert.Null(attr.Business);
    }

    [Fact]
    public void GenericAttribute_WithBusiness_SetsAllProperties()
    {
        // Act
        var attr = new QueueNameAttribute<DummyEntity>("Sales", "Updated");

        // Assert
        Assert.Equal(nameof(DummyEntity), attr.Entity);
        Assert.Equal("Updated", attr.Action);
        Assert.Equal("Sales", attr.Business);
    }

    private class DummyEntity { }
}
