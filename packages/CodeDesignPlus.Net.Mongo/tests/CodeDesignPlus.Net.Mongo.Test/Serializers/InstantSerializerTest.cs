using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Moq;
using CodeDesignPlus.Net.Mongo.Serializers;

namespace CodeDesignPlus.Net.Mongo.Test.Serializers;

public class InstantSerializerTest
{
    [Fact]
    public void Serialize_WritesCorrectMillisecondsSinceEpoch()
    {
        // Arrange
        var instant = Instant.FromUtc(2024, 1, 1, 0, 0);
        var serializer = new InstantSerializer();

        var mockWriter = new Mock<IBsonWriter>();
        BsonSerializationContext context = BsonSerializationContext.CreateRoot(mockWriter.Object);

        // Act
        serializer.Serialize(context, default, instant);

        // Assert
        var expectedDateTime = instant.ToDateTimeUtc();
        long expectedMilliseconds = (expectedDateTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        mockWriter.Verify(w => w.WriteDateTime(expectedMilliseconds), Times.Once);
    }

    [Fact]
    public void Deserialize_FromDateTime_ReturnsCorrectInstant()
    {
        // Arrange
        var serializer = new InstantSerializer();
        var expectedInstant = Instant.FromUtc(2024, 1, 1, 0, 0);
        var dateTime = expectedInstant.ToDateTimeUtc();
        long milliseconds = (dateTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;

        var mockReader = new Mock<IBsonReader>();
        mockReader.Setup(r => r.CurrentBsonType).Returns(BsonType.DateTime);
        mockReader.Setup(r => r.ReadDateTime()).Returns(milliseconds);

        BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(mockReader.Object);

        // Act
        var result = serializer.Deserialize(context, default);

        // Assert
        Assert.Equal(expectedInstant, result);
    }

    [Fact]
    public void Deserialize_FromString_ReturnsCorrectInstant()
    {
        // Arrange
        var serializer = new InstantSerializer();
        var instant = Instant.FromUtc(2024, 1, 1, 12, 34, 56) + Duration.FromNanoseconds(123456789);
        string instantString = instant.ToString("yyyy-MM-ddTHH:mm:ss.fffffffff'Z'", null);

        var mockReader = new Mock<IBsonReader>();
        mockReader.Setup(r => r.CurrentBsonType).Returns(BsonType.String);
        mockReader.Setup(r => r.ReadString()).Returns(instantString);

        BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(mockReader.Object);

        // Act
        var result = serializer.Deserialize(context, default);

        // Assert
        Assert.Equal(instant, result);
    }

    [Fact]
    public void Deserialize_InvalidString_ThrowsFormatException()
    {
        // Arrange
        var serializer = new InstantSerializer();
        var mockReader = new Mock<IBsonReader>();
        mockReader.Setup(r => r.CurrentBsonType).Returns(BsonType.String);
        mockReader.Setup(r => r.ReadString()).Returns("not-a-valid-instant");

        BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(mockReader.Object);

        // Act & Assert
        Assert.Throws<FormatException>(() => serializer.Deserialize(context, default));
    }

    [Fact]
    public void Deserialize_UnexpectedBsonType_ThrowsFormatException()
    {
        // Arrange
        var serializer = new InstantSerializer();
        var mockReader = new Mock<IBsonReader>();
        mockReader.Setup(r => r.CurrentBsonType).Returns(BsonType.Int32);

        BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(mockReader.Object);

        // Act & Assert
        Assert.Throws<FormatException>(() => serializer.Deserialize(context, default));
    }

    [Fact]
    public void IBsonSerializer_Deserialize_CallsTypedDeserialize()
    {
        // Arrange
        var serializer = new InstantSerializer();
        var instant = Instant.FromUtc(2024, 1, 1, 0, 0);
        var dateTime = instant.ToDateTimeUtc();
        long milliseconds = (dateTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;

        var mockReader = new Mock<IBsonReader>();
        mockReader.Setup(r => r.CurrentBsonType).Returns(BsonType.DateTime);
        mockReader.Setup(r => r.ReadDateTime()).Returns(milliseconds);

        BsonDeserializationContext context = BsonDeserializationContext.CreateRoot(mockReader.Object);

        // Act
        var result = ((IBsonSerializer)serializer).Deserialize(context, default);

        // Assert
        Assert.IsType<Instant>(result);
        Assert.Equal(instant, (Instant)result);
    }

    [Fact]
    public void IBsonSerializer_Serialize_CallsTypedSerialize()
    {
        // Arrange
        var serializer = new InstantSerializer();
        var instant = Instant.FromUtc(2024, 1, 1, 0, 0);

        var mockWriter = new Mock<IBsonWriter>();
        BsonSerializationContext context = BsonSerializationContext.CreateRoot(mockWriter.Object);

        // Act
        ((IBsonSerializer)serializer).Serialize(context, default, instant);

        // Assert
        var expectedDateTime = instant.ToDateTimeUtc();
        long expectedMilliseconds = (expectedDateTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        mockWriter.Verify(w => w.WriteDateTime(expectedMilliseconds), Times.Once);
    }

    [Fact]
    public void ValueType_ReturnsInstantNullableType()
    {
        // Arrange
        var serializer = new InstantSerializer();

        // Act
        var valueType = serializer.ValueType;

        // Assert
        Assert.Equal(typeof(Instant), valueType);
    }
}
