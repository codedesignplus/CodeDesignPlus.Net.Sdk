using CodeDesignPlus.Net.Kafka.Serializer;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using Confluent.Kafka;

namespace CodeDesignPlus.Net.Kafka.Test.Serializer;

public class JsonSystemTextSerializerTest
{
    private readonly JsonSystemTextSerializer<UserCreatedEvent> _serializer = new JsonSystemTextSerializer<UserCreatedEvent>();

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
        var jsonData = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(expected);
        var isNull = false;

        // Act
        var result = _serializer.Deserialize(jsonData, isNull, new SerializationContext());

        // Assert
        Assert.Equal(expected.Username, result.Username);
        Assert.Equal(expected.Names, result.Names);
    }
}
