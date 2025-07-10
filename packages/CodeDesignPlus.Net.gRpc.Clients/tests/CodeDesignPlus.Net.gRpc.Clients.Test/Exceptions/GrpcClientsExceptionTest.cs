
namespace CodeDesignPlus.Net.gRpc.Clients.Test.Exceptions;

public class GrpcClientsExceptionTest
{
    [Fact]
    public void DefaultConstructor_SetsEmptyErrors()
    {
        var ex = new GrpcClientsException();
        Assert.NotNull(ex.Errors);
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void Constructor_WithErrors_SetsErrors()
    {
        var errors = new List<string> { "Error1", "Error2" };
        var ex = new GrpcClientsException(errors);
        Assert.Equal(errors, ex.Errors);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessageAndEmptyErrors()
    {
        var message = "Test message";
        var ex = new GrpcClientsException(message);
        Assert.Equal(message, ex.Message);
        Assert.NotNull(ex.Errors);
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void Constructor_WithMessageAndErrors_SetsProperties()
    {
        var message = "Test message";
        var errors = new List<string> { "Error1" };
        var ex = new GrpcClientsException(message, errors);
        Assert.Equal(message, ex.Message);
        Assert.Equal(errors, ex.Errors);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        var message = "Test message";
        var inner = new InvalidOperationException("Inner");
        var ex = new GrpcClientsException(message, inner);
        Assert.Equal(message, ex.Message);
        Assert.Same(inner, ex.InnerException);
        Assert.NotNull(ex.Errors);
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void Constructor_WithMessageErrorsAndInnerException_SetsAllProperties()
    {
        var message = "Test message";
        var errors = new List<string> { "Error1", "Error2" };
        var inner = new Exception("Inner");
        var ex = new GrpcClientsException(message, errors, inner);
        Assert.Equal(message, ex.Message);
        Assert.Equal(errors, ex.Errors);
        Assert.Same(inner, ex.InnerException);
    }
}
