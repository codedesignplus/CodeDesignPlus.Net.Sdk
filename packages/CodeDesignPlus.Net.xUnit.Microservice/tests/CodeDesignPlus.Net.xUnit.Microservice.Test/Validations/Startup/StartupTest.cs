
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.xUnit.Microservice.Validations.Startup;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Validations.Startup;

/// <summary>
/// A class for validating startup services.
/// </summary>
public class StartupTest
{
    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Theory]
    [StartupValidation]
    public void Sturtup_CheckNotThrowException(IStartupServices startup, Exception exception)
    {
        // Assert
        Assert.NotNull(startup);
        Assert.Null(exception);
    }
}