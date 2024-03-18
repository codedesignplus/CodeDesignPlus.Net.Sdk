using CodeDesignPlus.Net.Redis.Diagnostics.Options;
using CodeDesignPlus.Net.Redis.Diagnostics.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Redis.Diagnostics.Test;

public class RedisDiagnosticsServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<RedisDiagnosticsService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisDiagnosticsOptions());

        // Act
        var actual = await new RedisDiagnosticsService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
