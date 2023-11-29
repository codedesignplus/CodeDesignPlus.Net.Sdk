using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class EventSourcingNotImplementedExceptionTest
{
    /// <summary>
    /// Valida el constructor por defecto de la excepción
    /// </summary>
    [Fact]
    public void Constructor_WithoutArguments_Exception()
    {
        // Arrange & Act
        var exception = new EventSourcingNotImplementedException();

        // Assert
        Assert.NotEmpty(exception.Message);
        Assert.Null(exception.InnerException);
    }

    /// <summary>
    /// Valida el constructor con el mensaje
    /// </summary>
    [Fact]
    public void Constructor_Message_Exception()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new EventSourcingNotImplementedException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    /// <summary>
    /// Valida el constructor con el mensaje y la excepción interna
    /// </summary>
    [Fact]
    public void Constructor_InnerException_Exception()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new InvalidOperationException("The operation is invalid");

        // Act
        var exception = new EventSourcingNotImplementedException(message, innerException);

        // Assert
        Assert.Equal(innerException, exception.InnerException);
        Assert.Equal(message, exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    /// <summary>
    /// Valida el constructor con el mensaje y la excepción interna
    /// </summary>
    [Fact]
    public void Constructor_Serealization_Exception()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new InvalidOperationException("The operation is invalid");

        // Act
        var exception = new EventSourcingNotImplementedException(message, innerException);

        var serialize = JsonConvert.SerializeObject(exception);

        var result = JsonConvert.DeserializeObject(serialize, typeof(EventSourcingNotImplementedException)) as EventSourcingNotImplementedException;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.InnerException);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Equal(message, exception.Message);
    }
}