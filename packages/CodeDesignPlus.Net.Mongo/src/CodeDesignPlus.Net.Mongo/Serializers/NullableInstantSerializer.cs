using NodaTime.Text;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializer for <see cref="Instant"/>.
/// </summary>
public class NullableInstantSerializer : IBsonSerializer<Instant?>
{
    private static readonly InstantPattern Pattern = InstantPattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss.fffffffff'Z'");
    public Type ValueType => typeof(Instant?);

    /// <summary>
    /// Serialize the <see cref="Instant"/> to a string.
    /// </summary>
    /// <param name="context">Context for the serialization.</param>
    /// <param name="args">Arguments for the serialization.</param>
    /// <param name="value">The value to serialize.</param>
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Instant? value)
    {
        if (!value.HasValue)
        {
            context.Writer.WriteNull();
            return;
        }

        var dateTime = value.Value.ToDateTimeUtc();

        long millisecondsSinceEpoch = (dateTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;

        context.Writer.WriteDateTime(millisecondsSinceEpoch);
    }

    /// <summary>
    /// Deserialize the <see cref="Instant"/> from a string.
    /// </summary>
    /// <param name="context">Context for the deserialization.</param>
    /// <param name="args">Arguments for the deserialization.</param>
    /// <returns>The deserialized value.</returns>
    /// <exception cref="FormatException">Thrown when the value is not a valid <see cref="Instant"/>.</exception>
    public Instant? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null;
        }

        if (context.Reader.CurrentBsonType == BsonType.DateTime)
        {
            long millisecondsSinceEpoch = context.Reader.ReadDateTime();

            var dateTime = DateTime.UnixEpoch.AddMilliseconds(millisecondsSinceEpoch);

            return Instant.FromDateTimeUtc(dateTime);
        }

        if (context.Reader.CurrentBsonType == BsonType.String)
        {
            var value = context.Reader.ReadString();
            var parseResult = Pattern.Parse(value);

            if (parseResult.Success)
                return parseResult.Value;

            throw new FormatException($"Error parsing Instant from: '{value}'");
        }

        throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when parsing Instant");
    }

    /// <summary>
    /// Deserialize the <see cref="Instant"/> from a string.
    /// </summary>
    /// <param name="context">Context for the deserialization.</param>
    /// <param name="args">Arguments for the deserialization.</param>
    /// <returns>The deserialized value.</returns>
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return this.Deserialize(context, args);
    }

    /// <summary>
    /// Serialize the <see cref="Instant"/> to a string.
    /// </summary>
    /// <param name="context">Context for the serialization.</param>
    /// <param name="args">Arguments for the serialization.</param>
    /// <param name="value">The value to serialize.</param>
    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        this.Serialize(context, args, (Instant?)value);
    }
}