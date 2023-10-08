using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Core.Test.Exceptions;

public class CoreExceptionTest
{
    [Fact]
    public void CoreException_DefaultValues_EmptyCoreException()
    {
        // Arrange & Act
        var exception = new CoreException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Core.Exceptions.CoreException' was thrown.", exception.Message);
    }

    [Fact]
    public void CoreException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new CoreException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Core.Exceptions.CoreException' was thrown.", exception.Message);
    }

    [Fact]
    public void CoreException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new CoreException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void CoreException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new CoreException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void CoreException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new CoreException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void CoreException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new CoreException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void CoreException_SerializationInfo_Call_Method()
    {
        // Arrange
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var message = Guid.NewGuid().ToString();

        var exception = new CoreException(message, errors);

        // Act 
        var serialize = JsonConvert.SerializeObject(exception);

        var deserialize = JsonConvert.DeserializeObject(serialize, typeof(CoreException)) as CoreException;

        //Assert
        Assert.NotNull(deserialize);
        Assert.Equal(exception.Message, deserialize.Message);
        Assert.Equal(exception.Errors, deserialize.Errors);
    }
}
