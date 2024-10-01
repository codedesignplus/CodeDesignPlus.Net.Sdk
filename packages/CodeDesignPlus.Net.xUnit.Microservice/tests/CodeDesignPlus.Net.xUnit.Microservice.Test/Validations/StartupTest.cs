namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations;

/// <summary>
/// A class for validating startup services.
/// </summary>
public class StartupTest
{
    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Theory]
    [Startup<Startup>]
    public void Sturtup_CheckNotThrowException(IStartupServices startup, Exception exception)
    {
        // Assert
        Assert.NotNull(startup);
        Assert.Null(exception);
    }
}