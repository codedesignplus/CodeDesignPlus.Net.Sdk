using CodeDesignPlus.Net.Kafka.Serializer;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Kafka.Test.Serializer;

public class JsonSystemTextSerializerTest
{
    private readonly JsonSystemTextSerializer<UserCreatedEvent> _serializer = new();

    [Fact]
    public void Serialize_ShouldReturnNull_WhenDataIsNull()
    {
        // Arrange
        UserCreatedEvent data = null!;

        // Act
        var result = _serializer.Serialize(data, new SerializationContext());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Serialize_ShouldReturnByteArray_WhenDataIsValid()
    {
        // Arrange
        var data = new UserCreatedEvent(Guid.NewGuid()) { Username = "Alice", Names = "Alice Rodriguez" };

        // Act
        var result = _serializer.Serialize(data, new SerializationContext());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<byte[]>(result);
    }

    [Fact]
    public void Deserialize_ShouldReturnDefaultValue_WhenDataIsNull()
    {
        // Arrange
        var data = Array.Empty<byte>();
        var isNull = true;

        // Act
        var result = _serializer.Deserialize(data, isNull, new SerializationContext());

        // Assert
        Assert.Equal(default, result);
    }

    [Fact]
    public void Deserialize_ShouldReturnObject_WhenDataIsValid()
    {
        // Arrange
        var expected = new UserCreatedEvent(Guid.NewGuid()) { Username = "Alice", Names = "Alice Rodriguez" };
        var jsonData = JsonConvert.SerializeObject(expected);
        var @bytes = Encoding.UTF8.GetBytes(jsonData);
        var isNull = false;

        // Act
        var result = _serializer.Deserialize(@bytes, isNull, new SerializationContext());

        // Assert
        Assert.Equal(expected.Username, result.Username);
        Assert.Equal(expected.Names, result.Names);
        Assert.Equal(expected.EventId, result.EventId);
        Assert.Equal(expected.AggregateId, result.AggregateId);
        Assert.Equal(expected.Birthdate.ToString(), result.Birthdate.ToString());
        Assert.Equal(expected.Lastnames, result.Lastnames);
        Assert.Equal(expected.OccurredAt.ToString(), result.OccurredAt.ToString());
        Assert.Equal(expected.Metadata, result.Metadata);

    }
}
