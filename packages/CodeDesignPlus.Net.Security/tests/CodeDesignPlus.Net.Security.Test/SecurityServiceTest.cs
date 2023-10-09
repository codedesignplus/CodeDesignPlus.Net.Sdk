using CodeDesignPlus.Net.Security.Options;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Security.Test;

public class SecurityServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<SecurityService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new SecurityOptions());

        // Act
        var actual = await new SecurityService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
