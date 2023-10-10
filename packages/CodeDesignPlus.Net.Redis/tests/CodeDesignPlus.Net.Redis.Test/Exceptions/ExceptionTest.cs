using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Redis.Test.Exceptions;

public class RedisExceptionTest
{
    [Fact]
    public void RedisException_DefaultValues_EmptyRedisException()
    {
        // Arrange & Act
        var exception = new RedisException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Redis.Exceptions.RedisException' was thrown.", exception.Message);
    }

    [Fact]
    public void RedisException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new RedisException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Redis.Exceptions.RedisException' was thrown.", exception.Message);
    }

    [Fact]
    public void RedisException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new RedisException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void RedisException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new RedisException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void RedisException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new RedisException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void RedisException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new RedisException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void RedisException_SerializationInfo_Call_Method()
    {
        // Arrange
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var message = Guid.NewGuid().ToString();

        var exception = new RedisException(message, errors);

        // Act 
        var serialize = JsonConvert.SerializeObject(exception);

        var deserialize = JsonConvert.DeserializeObject(serialize, typeof(RedisException)) as RedisException;

        //Assert
        Assert.NotNull(deserialize);
        Assert.Equal(exception.Message, deserialize.Message);
        Assert.Equal(exception.Errors, deserialize.Errors);
    }
}
