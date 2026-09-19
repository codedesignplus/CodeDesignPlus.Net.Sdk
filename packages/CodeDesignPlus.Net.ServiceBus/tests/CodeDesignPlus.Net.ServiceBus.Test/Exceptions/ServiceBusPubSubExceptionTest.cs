namespace CodeDesignPlus.Net.ServiceBus.Test.Exceptions;

public class ServiceBusPubSubExceptionTest
{
    private static readonly string[] Errors = ["error 1", "error 2"];

    [Fact]
    public void Constructor_Default_ErrorsIsNull()
    {
        var exception = new ServiceBusPubSubException();

        Assert.Null(exception.Errors);
    }

    [Fact]
    public void Constructor_Errors_SetsErrors()
    {
        var exception = new ServiceBusPubSubException(Errors);

        Assert.Equal(Errors, exception.Errors);
    }

    [Fact]
    public void Constructor_Message_SetsMessage()
    {
        var exception = new ServiceBusPubSubException("The message");

        Assert.Equal("The message", exception.Message);
    }

    [Fact]
    public void Constructor_MessageAndErrors_SetsBoth()
    {
        var exception = new ServiceBusPubSubException("The message", Errors);

        Assert.Equal("The message", exception.Message);
        Assert.Equal(Errors, exception.Errors);
    }

    [Fact]
    public void Constructor_MessageAndInnerException_SetsBoth()
    {
        var inner = new InvalidOperationException("inner");

        var exception = new ServiceBusPubSubException("The message", inner);

        Assert.Equal("The message", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Constructor_MessageErrorsAndInnerException_SetsAll()
    {
        var inner = new InvalidOperationException("inner");

        var exception = new ServiceBusPubSubException("The message", Errors, inner);

        Assert.Equal("The message", exception.Message);
        Assert.Equal(Errors, exception.Errors);
        Assert.Same(inner, exception.InnerException);
    }
}
