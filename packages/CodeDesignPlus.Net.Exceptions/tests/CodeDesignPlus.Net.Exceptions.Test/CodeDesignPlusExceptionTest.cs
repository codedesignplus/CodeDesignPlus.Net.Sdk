namespace CodeDesignPlus.Net.Exceptions.Test;

public class CodeDesignPlusExceptionTest
{
    [Fact]
    public void CodeDesignPlusException_DefaultValues_EmptyCodeDesignPlusException()
    {
        // Arrange
        var code = Guid.NewGuid().ToString();

        // Act
        var exception = new CodeDesignPlusException(Layer.Domain, code);

        // Assert 
        Assert.NotNull(exception);
        Assert.Equal(code, exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
    }

    [Fact]
    public void CodeDesignPlusException_CheckMessage()
    {
        // Arrange 
        var code = Guid.NewGuid().ToString();
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new CodeDesignPlusException(Layer.Domain, code, message);

        // Assert         
        Assert.NotNull(exception);
        Assert.Equal(code, exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void CodeDesignPlusException_CheckMessageAndInnerException()
    {
        // Arrange 
        var code = Guid.NewGuid().ToString();
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new CodeDesignPlusException(Layer.Domain, code, message, innerException);

        // Assert 
        Assert.NotNull(exception);
        Assert.Equal(code, exception.Code);
        Assert.Equal(Layer.Domain, exception.Layer);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }
}
