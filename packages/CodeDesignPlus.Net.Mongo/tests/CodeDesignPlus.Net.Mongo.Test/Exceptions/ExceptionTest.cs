﻿namespace CodeDesignPlus.Net.Mongo.Test.Exceptions;

public class MongoExceptionTest
{
    [Fact]
    public void MongoException_DefaultValues_EmptyMongoException()
    {
        // Arrange & Act
        var exception = new MongoException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Mongo.Exceptions.MongoException' was thrown.", exception.Message);
    }

    [Fact]
    public void MongoException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new MongoException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Mongo.Exceptions.MongoException' was thrown.", exception.Message);
    }

    [Fact]
    public void MongoException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new MongoException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void MongoException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new MongoException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void MongoException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new MongoException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void MongoException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new MongoException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void ThrowIfNull_ArgumentNotNull_ThrowsMongoException()
    {
        // Arrange
        object argument = new();

        var message = Guid.NewGuid().ToString();

        // Act & Assert
        MongoException.ThrowIfNull(argument, message);
    }

    [Fact]
    public void ThrowIfNull_ArgumentNull_ThrowsMongoException()
    {
        // Arrange
        object argument = null!;

        var message = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = Assert.Throws<MongoException>(() => MongoException.ThrowIfNull(argument, message));

        Assert.Equal(message, exception.Message);
    }
}
