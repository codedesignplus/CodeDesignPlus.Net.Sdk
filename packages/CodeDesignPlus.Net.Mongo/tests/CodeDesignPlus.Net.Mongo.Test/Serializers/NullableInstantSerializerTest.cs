using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using CodeDesignPlus.Net.Mongo.Serializers;

namespace CodeDesignPlus.Net.Mongo.Test.Serializers;

public class NullableInstantSerializerTest
{
    private readonly NullableInstantSerializer _serializer = new();

    [Fact]
    public void Serialize_Null_WritesNull()
    {
        var writer = new BsonDocumentWriter([]);
        writer.WriteStartDocument();
        writer.WriteName("value");

        _serializer.Serialize(BsonSerializationContext.CreateRoot(writer), default, null);

        writer.WriteEndDocument();
        var doc = writer.Document;
        Assert.True(doc.Contains("value"));
        Assert.True(doc["value"].IsBsonNull);
    }

    [Fact]
    public void Serialize_Instant_WritesDateTime()
    {
        var instant = Instant.FromUtc(2024, 1, 1, 12, 0, 0);
        var writer = new BsonDocumentWriter([]);
        writer.WriteStartDocument();
        writer.WriteName("value");

        _serializer.Serialize(BsonSerializationContext.CreateRoot(writer), default, instant);

        writer.WriteEndDocument();
        var doc = writer.Document;
        Assert.True(doc.Contains("value"));
        Assert.True(doc["value"].IsValidDateTime);
        var expected = instant.ToDateTimeUtc();
        Assert.Equal(expected, doc["value"].ToUniversalTime());
    }

    [Fact]
    public void Deserialize_Null_ReturnsNull()
    {
        var doc = new BsonDocument { { "value", BsonNull.Value } };
        var reader = new BsonDocumentReader(doc);
        reader.ReadStartDocument();
        reader.ReadName("value");

        var result = _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default);

        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_DateTime_ReturnsInstant()
    {
        var instant = Instant.FromUtc(2024, 1, 1, 12, 0, 0);
        var dateTime = instant.ToDateTimeUtc();
        var doc = new BsonDocument { { "value", dateTime } };
        var reader = new BsonDocumentReader(doc);
        reader.ReadStartDocument();
        reader.ReadName("value");

        var result = _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default);

        Assert.NotNull(result);
        Assert.Equal(instant, result.Value);
    }

    [Fact]
    public void Deserialize_String_ReturnsInstant()
    {
        var instant = Instant.FromUtc(2024, 1, 1, 12, 0, 0);
        var pattern = "yyyy-MM-ddTHH:mm:ss.fffffffff'Z'";
        var value = instant.ToString(pattern, null);
        var doc = new BsonDocument { { "value", value } };
        var reader = new BsonDocumentReader(doc);
        reader.ReadStartDocument();
        reader.ReadName("value");

        var result = _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default);

        Assert.NotNull(result);
        Assert.Equal(instant, result.Value);
    }

    [Fact]
    public void Deserialize_String_Invalid_ThrowsFormatException()
    {
        var doc = new BsonDocument { { "value", "invalid-instant" } };
        var reader = new BsonDocumentReader(doc);
        reader.ReadStartDocument();
        reader.ReadName("value");

        Assert.Throws<FormatException>(() =>
            _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default)
        );
    }

    [Fact]
    public void Deserialize_UnexpectedType_ThrowsFormatException()
    {
        var doc = new BsonDocument { { "value", 123 } };
        var reader = new BsonDocumentReader(doc);
        reader.ReadStartDocument();
        reader.ReadName("value");

        Assert.Throws<FormatException>(() =>
            _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default)
        );
    }

    [Fact]
    public void ValueType_ReturnsInstantNullableType()
    {
        // Arrange
        var serializer = new NullableInstantSerializer();

        // Act
        var valueType = serializer.ValueType;

        // Assert
        Assert.Equal(typeof(Instant?), valueType);
    }
}
