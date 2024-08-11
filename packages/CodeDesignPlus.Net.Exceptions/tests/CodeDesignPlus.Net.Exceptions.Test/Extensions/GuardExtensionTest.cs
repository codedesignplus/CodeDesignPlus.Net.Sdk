using CodeDesignPlus.Net.Exceptions.Extensions;

namespace CodeDesignPlus.Net.Exceptions.Test.Extensions;

public class GuardExtensionTest
{
    [Fact]
    public void GetCode_ValidMessage_ReturnsCode()
    {
        // Arrange
        var message = "12345: This is a test message";

        // Act
        var code = message.GetCode();

        // Assert
        Assert.Equal("12345", code);
    }

    [Fact]
    public void GetMessage_ValidMessage_ReturnsMessage()
    {
        // Arrange
        var message = "12345: This is a test message";

        // Act
        var result = message.GetMessage();

        // Assert
        Assert.Equal("This is a test message", result);
    }
}
