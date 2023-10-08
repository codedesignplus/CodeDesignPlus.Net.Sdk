using Moq;

namespace CodeDesignPlus.Net.Core.Test;

public class CoreServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<CoreService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new CoreOptions());

        // Act
        var actual = await new CoreService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
