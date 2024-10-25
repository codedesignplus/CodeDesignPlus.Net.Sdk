namespace CodeDesignPlus.Net.Vault.Test.Exceptions;

public class VaultExceptionTest
{
    [Fact]
    public void VaultException_DefaultValues_EmptyVaultException()
    {
        // Arrange & Act
        var exception = new VaultException();

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Vault.Exceptions.VaultException' was thrown.", exception.Message);
    }

    [Fact]
    public void VaultException_Errors()
    {
        // Arrange & Act
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var exception = new VaultException(errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal("Exception of type 'CodeDesignPlus.Net.Vault.Exceptions.VaultException' was thrown.", exception.Message);
    }

    [Fact]
    public void VaultException_CheckMessage()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();

        // Act
        var exception = new VaultException(message);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void VaultException_CheckMessageAndErrors()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };

        // Act
        var exception = new VaultException(message, errors);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void VaultException_CheckMessageAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new VaultException(message, innerException);

        // Assert 
        Assert.Null(exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }

    [Fact]
    public void VaultException_CheckMessageErrorsAndInnerException()
    {
        // Arrange 
        var message = Guid.NewGuid().ToString();
        var errors = new List<string>() { Guid.NewGuid().ToString() };
        var innerException = new System.Exception("This is inner exception");

        // Act
        var exception = new VaultException(message, errors, innerException);

        // Assert 
        Assert.Equal(errors, exception.Errors);
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);

    }
}
