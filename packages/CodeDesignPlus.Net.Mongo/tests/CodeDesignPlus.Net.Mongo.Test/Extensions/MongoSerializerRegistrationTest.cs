using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Serializers;
using Xunit;
using MongoDB.Bson;

namespace CodeDesignPlus.Net.Mongo.Test.Extensions;

public class MongoSerializerRegistrationTest
{
    [Fact]
    public void RegisterSerializers_ShouldNotThrow_WhenCalledMultipleTimes()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
        {
            MongoSerializerRegistration.RegisterSerializers();
            MongoSerializerRegistration.RegisterSerializers();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void RegisterSerializers_ShouldRegisterGuidSerializer()
    {
        // Arrange
        MongoSerializerRegistration.RegisterSerializers();

        // Act
        var serializer = BsonSerializer.LookupSerializer<Guid>();

        // Assert
        Assert.IsType<GuidSerializer>(serializer);
        var guidSerializer = (GuidSerializer)serializer;
        Assert.Equal(GuidRepresentation.Standard, guidSerializer.GuidRepresentation);
    }

    [Fact]
    public void RegisterSerializers_ShouldRegisterInstantSerializer()
    {
        // Arrange
        MongoSerializerRegistration.RegisterSerializers();

        // Act
        var serializer = BsonSerializer.LookupSerializer<NodaTime.Instant>();

        // Assert
        Assert.IsType<InstantSerializer>(serializer);
    }

    [Fact]
    public void RegisterSerializers_ShouldRegisterNullableInstantSerializer()
    {
        // Arrange
        MongoSerializerRegistration.RegisterSerializers();

        // Act
        var serializer = BsonSerializer.LookupSerializer<NodaTime.Instant?>();

        // Assert
        Assert.IsType<NullableInstantSerializer>(serializer);
    }
}
