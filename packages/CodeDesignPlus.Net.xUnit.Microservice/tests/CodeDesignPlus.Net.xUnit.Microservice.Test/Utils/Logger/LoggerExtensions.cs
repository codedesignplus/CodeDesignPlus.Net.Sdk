namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Utils.Logger;

/// <summary>
/// Tests for the LoggerExtensions class.
/// </summary>
public class LoggerExtensionsTests
{
    [Fact]
    public void VerifyLogging_ShouldVerifyLogMessage()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LoggerExtensionsTests>>();
        const string expectedMessage = "Test message";
        var expectedLogLevel = LogLevel.Information;

        // Act
        loggerMock.Object.LogInformation(expectedMessage);

        // Assert
        loggerMock.VerifyLogging(expectedMessage, expectedLogLevel);
    }

    [Fact]
    public void VerifyLoggerWasCalled_ShouldVerifyLogMethodWasCalled()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LoggerExtensionsTests>>();

        // Act
        loggerMock.Object.LogInformation("Test message");

        // Assert
        loggerMock.VerifyLoggerWasCalled();
    }
}