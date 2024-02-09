using Newtonsoft.Json;

namespace CodeDesignPlus.Net.PubSub.Test.Exceptions;

public class PubSubExceptionTest
{
    [Fact]
    public void PubSubException_DefaultValues_EmptyPubSubException()
    {
        // Arrange & Act
        var exception = new PubSubException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.PubSub.Exceptions.PubSubException' was thrown.", exception.Message);
    }

    [Fact]
    public void PubSubException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new PubSubException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.PubSub.Exceptions.PubSubException' was thrown.", exception.Message);
    }

    [Fact]
    public void PubSubException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new PubSubException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void PubSubException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new PubSubException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void PubSubException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new PubSubException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void PubSubException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new PubSubException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

}
