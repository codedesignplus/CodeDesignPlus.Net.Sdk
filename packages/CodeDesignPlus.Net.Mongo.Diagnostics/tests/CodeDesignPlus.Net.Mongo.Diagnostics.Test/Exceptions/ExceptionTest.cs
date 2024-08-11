namespace CodeDesignPlus.Net.Mongo.Diagnostics.Test.Exceptions;

public class MongoDiagnosticsExceptionTest
{
    [Fact]
    public void MongoDiagnosticsException_DefaultValues_EmptyMongoDiagnosticsException()
    {
        // Arrange & Act
        var exception = new MongoDiagnosticsException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Mongo.Diagnostics.Exceptions.MongoDiagnosticsException' was thrown.", exception.Message);
    }

    [Fact]
    public void MongoDiagnosticsException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new MongoDiagnosticsException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Mongo.Diagnostics.Exceptions.MongoDiagnosticsException' was thrown.", exception.Message);
    }

    [Fact]
    public void MongoDiagnosticsException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new MongoDiagnosticsException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void MongoDiagnosticsException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new MongoDiagnosticsException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void MongoDiagnosticsException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new MongoDiagnosticsException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void MongoDiagnosticsException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new MongoDiagnosticsException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }
}
