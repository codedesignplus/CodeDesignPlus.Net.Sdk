using CodeDesignPlus.Net.Serializers.Test.Helpers;
using CodeDesignPlus.Net.Serializers.Test.Helpers.DomainEvents;
using CodeDesignPlus.Net.Serializers;
using System.Globalization;

namespace CodeDesignPlus.Net.Serializers.Test;

public class JsonSerializerTest
{
    [Fact]
    public void Serialize_ShouldReturnValidJsonString()
    {
        // Arrange
        var userDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid())
        {
            Name = "John",
            Email = "john@outlook.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var json = JsonSerializer.Serialize(userDomainEvent);



        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.True(JsonValidator.IsValidJson(json));
    }

    [Fact]
    public void Serialize_ShouldReturnValidJsonString_WithFormatting()
    {
        // Arrange
        var userDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid())
        {
            Name = "John",
            Email = "john@outlook.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var json = JsonSerializer.Serialize(userDomainEvent, Formatting.Indented);

        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.True(JsonValidator.IsValidJson(json));
    }

    [Fact]
    public void Serialize_ShouldReturnValidJsonString_WithJsonSerializerSettings()
    {
        var userDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid())
        {
            Name = "John",
            Email = "john@outlook.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var json = JsonSerializer.Serialize(userDomainEvent, new JsonSerializerSettings { Formatting = Formatting.Indented });

        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.True(JsonValidator.IsValidJson(json));
    }


    [Fact]
    public void Serialize_ShouldReturnValidJsonString_WithFormatingAndJsonSerializerSettings()
    {
        var userDomainEvent = new UserCreatedDomainEvent(Guid.NewGuid())
        {
            Name = "John",
            Email = "john@outlook.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var json = JsonSerializer.Serialize(userDomainEvent, Formatting.Indented, new JsonSerializerSettings
        {
            Culture = CultureInfo.InvariantCulture,
        });

        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.True(JsonValidator.IsValidJson(json));
    }

    [Fact]
    public void Deserialize_ShouldReturnValidObject()
    {
        // Arrange
        var json = "{\"Name\":\"John\",\"Email\":\"john@outlook.com\",\"Password\":\"123456\",\"Role\":\"Admin\"}";

        // Act
        var @event = JsonSerializer.Deserialize<UserCreatedDomainEvent>(json);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<UserCreatedDomainEvent>(@event);
        Assert.Equal("John", @event.Name);
        Assert.Equal("john@outlook.com", @event.Email);
        Assert.Equal("123456", @event.Password);
        Assert.Equal("Admin", @event.Role);
    }

    [Fact]
    public void Deserialize_ShouldReturnValidObject_WithCustomType()
    {
        // Arrange
        var json = "{\"Name\":\"John\",\"Email\":\"john@outlook.com\",\"Password\":\"123456\",\"Role\":\"Admin\"}";

        // Act
        var @event = JsonSerializer.Deserialize(json, typeof(UserCreatedDomainEvent)) as UserCreatedDomainEvent;

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<UserCreatedDomainEvent>(@event);
        Assert.Equal("John", @event.Name);
        Assert.Equal("john@outlook.com", @event.Email);
        Assert.Equal("123456", @event.Password);
        Assert.Equal("Admin", @event.Role);
    }

    [Fact]
    public void Deserialize_ShouldReturnValidObject_WithJsonSerializerSettings()
    {
        // Arrange
        var json = "{\r\n  \"Name\": \"John\",\r\n  \"Email\": \"john@outlook.com\",\r\n  \"Password\": \"123456\",\r\n  \"Role\": \"Admin\"\r\n}";

        // Act
        var @event = JsonSerializer.Deserialize<UserCreatedDomainEvent>(json, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        });

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<UserCreatedDomainEvent>(@event);
        Assert.Equal("John", @event.Name);
        Assert.Equal("john@outlook.com", @event.Email);
        Assert.Equal("123456", @event.Password);
        Assert.Equal("Admin", @event.Role);
    }


    [Fact]
    public void Deserialize_ShouldReturnValidObject_WithTypeAndJsonSerializerSettings()
    {
        // Arrange
        var json = "{\r\n  \"Name\": \"John\",\r\n  \"Email\": \"john@outlook.com\",\r\n  \"Password\": \"123456\",\r\n  \"Role\": \"Admin\"\r\n}";

        // Act
        var @event = JsonSerializer.Deserialize(json, typeof(UserCreatedDomainEvent), new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        }) as UserCreatedDomainEvent;

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<UserCreatedDomainEvent>(@event);
        Assert.Equal("John", @event.Name);
        Assert.Equal("john@outlook.com", @event.Email);
        Assert.Equal("123456", @event.Password);
        Assert.Equal("Admin", @event.Role);
    }
}