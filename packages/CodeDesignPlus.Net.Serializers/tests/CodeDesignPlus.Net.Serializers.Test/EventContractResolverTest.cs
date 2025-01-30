using CodeDesignPlus.Net.Serializers.Test.Helpers.DomainEvents;
using CodeDesignPlus.Net.Serializers;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Serializers.Test;

public class EventContractResolverTest
{
    [Fact]
    public void SerializerAndDeserializer_WithSkipProperties_EqualsObject()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var occurredAt = SystemClock.Instance.GetCurrentInstant();
        var metadata = new Dictionary<string, object>()
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        var userCreatedDomainEvent = new UserCreatedDomainEvent(idAggregate, eventId, occurredAt, metadata)
        {
            Name = "antony lopez",
            Email = "antony.lopez@gmail.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new EventContractResolver([])
        };

        var json = JsonSerializer.Serialize(userCreatedDomainEvent, jsonSerializerSettings);

        var userCreatedDomainEventDeserialized = JsonSerializer.Deserialize<UserCreatedDomainEvent>(json, jsonSerializerSettings);

        // Assert
        Assert.NotNull(userCreatedDomainEventDeserialized);
        Assert.Equal(userCreatedDomainEvent.Name, userCreatedDomainEventDeserialized.Name);
        Assert.Equal(userCreatedDomainEvent.Email, userCreatedDomainEventDeserialized.Email);
        Assert.Equal(userCreatedDomainEvent.Password, userCreatedDomainEventDeserialized.Password);
        Assert.Equal(userCreatedDomainEvent.Role, userCreatedDomainEventDeserialized.Role);
        Assert.Equal(userCreatedDomainEvent.EventId, userCreatedDomainEventDeserialized.EventId);
        Assert.Equal(userCreatedDomainEvent.OccurredAt, userCreatedDomainEventDeserialized.OccurredAt);
        Assert.Equal(userCreatedDomainEvent.Metadata, userCreatedDomainEventDeserialized.Metadata);
    }

    [Fact]
    public void SerializerAndDeserializer_WithProperties_DiferentObjects()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var occurredAt = SystemClock.Instance.GetCurrentInstant();
        var metadata = new Dictionary<string, object>()
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        var userCreatedDomainEvent = new UserCreatedDomainEvent(idAggregate, eventId, occurredAt, metadata)
        {
            Name = "antony lopez",
            Email = "antony.lopez@gmail.com",
            Password = "123456",
            Role = "Admin"
        };

        // Act
        var jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new EventContractResolver()
        };

        var json = JsonSerializer.Serialize(userCreatedDomainEvent, jsonSerializerSettings);

        var userCreatedDomainEventDeserialized = JsonSerializer.Deserialize<UserCreatedDomainEvent>(json, jsonSerializerSettings);

        // Assert
        Assert.NotNull(userCreatedDomainEventDeserialized);
        Assert.Equal(userCreatedDomainEvent.Name, userCreatedDomainEventDeserialized.Name);
        Assert.Equal(userCreatedDomainEvent.Email, userCreatedDomainEventDeserialized.Email);
        Assert.Equal(userCreatedDomainEvent.Password, userCreatedDomainEventDeserialized.Password);
        Assert.Equal(userCreatedDomainEvent.Role, userCreatedDomainEventDeserialized.Role);
        Assert.NotEqual(userCreatedDomainEvent.EventId, userCreatedDomainEventDeserialized.EventId);
        Assert.NotEqual(userCreatedDomainEvent.OccurredAt, userCreatedDomainEventDeserialized.OccurredAt);
        Assert.NotEqual(userCreatedDomainEvent.Metadata, userCreatedDomainEventDeserialized.Metadata);
    }

    [Fact]
    public void ResolvePropertyName_PropertyNameIsNull_ReturnPropertyName()
    {
        // Arrange
        var eventContractResolver = new EventContractResolver();

        var methodInfo = typeof(EventContractResolver).GetMethod("ResolvePropertyName", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var propertyName = methodInfo!.Invoke(eventContractResolver, [null!]);

        // Assert
        Assert.Null(propertyName);
    }

    [Fact]
    public void CreateProperty_IsNotAssignableFrom_ReturnProperty()
    {
        // Arrange
        var eventContractResolver = new EventContractResolver();
        var memberInfo = typeof(FakeDomainEvent).GetProperty(nameof(FakeDomainEvent.Name));
        var memberSerialization = MemberSerialization.OptIn;

        var methodInfo = typeof(EventContractResolver).GetMethod("CreateProperty", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var property = methodInfo!.Invoke(eventContractResolver, [memberInfo!, memberSerialization]);

        // Assert
        Assert.NotNull(property);
        Assert.IsType<JsonProperty>(property);
        Assert.Equal(nameof(FakeDomainEvent.Name).ToLower(), (property as JsonProperty)!.PropertyName);
    }
}