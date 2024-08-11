using CodeDesignPlus.Net.Logger.Options;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Logger.Test.Options;

public class LoggerOptionsTest
{
    [Fact]
    public void LoggerOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new LoggerOptions()
        {
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void LoggerOptions_InvalidValues_Invalid()
    {
        // Arrange
        var options = new LoggerOptions()
        {
            Enable = true,
            OTelEndpoint = "InvalidUrl"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
    }

    [Fact]
    public void LoggerOptions_ValidValues_Valid()
    {
        // Arrange
        var options = new LoggerOptions()
        {
            Enable = true,
            OTelEndpoint = "https://localhost:4317"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
