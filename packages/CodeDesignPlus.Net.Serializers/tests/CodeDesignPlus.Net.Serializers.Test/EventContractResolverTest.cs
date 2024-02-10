
using Xunit;
using CodeDesignPlus.Net.Serializers;
using CodeDesignPlus.Net.Core.Abstractions;
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
        var occurredAt = DateTime.Now;
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

        var json = JsonConvert.SerializeObject(userCreatedDomainEvent, jsonSerializerSettings);

        var userCreatedDomainEventDeserialized = JsonConvert.DeserializeObject<UserCreatedDomainEvent>(json, jsonSerializerSettings);

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
        var occurredAt = DateTime.Now;
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

        var json = JsonConvert.SerializeObject(userCreatedDomainEvent, jsonSerializerSettings);

        var userCreatedDomainEventDeserialized = JsonConvert.DeserializeObject<UserCreatedDomainEvent>(json, jsonSerializerSettings);

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
}