using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Redis.Diagnostics.Test.Exceptions;

public class RedisDiagnosticsExceptionTest
{
    [Fact]
    public void RedisDiagnosticsException_DefaultValues_EmptyRedisDiagnosticsException()
    {
        // Arrange & Act
        var exception = new RedisDiagnosticsException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Redis.Diagnostics.Exceptions.RedisDiagnosticsException' was thrown.", exception.Message);
    }

    [Fact]
    public void RedisDiagnosticsException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new RedisDiagnosticsException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Redis.Diagnostics.Exceptions.RedisDiagnosticsException' was thrown.", exception.Message);
    }

    [Fact]
    public void RedisDiagnosticsException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new RedisDiagnosticsException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void RedisDiagnosticsException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new RedisDiagnosticsException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void RedisDiagnosticsException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new RedisDiagnosticsException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void RedisDiagnosticsException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new RedisDiagnosticsException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void RedisDiagnosticsException_SerializationInfo_Call_Method()
    {
        // Arrange
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var message = Guid.NewGuid().ToString();

        var exception = new RedisDiagnosticsException(message, errors);

        // Act 
        var serialize = JsonConvert.SerializeObject(exception);

        var deserialize = JsonConvert.DeserializeObject(serialize, typeof(RedisDiagnosticsException)) as RedisDiagnosticsException;

        //Assert
        Assert.NotNull(deserialize);
        Assert.Equal(exception.Message, deserialize.Message);
        Assert.Equal(exception.Errors, deserialize.Errors);
    }
}
