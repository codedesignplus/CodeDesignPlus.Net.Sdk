using CodeDesignPlus.Net.Observability.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Observability.Test;

public class ObservabilityServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<ObservabilityService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new ObservabilityOptions());

        // Act
        var actual = await new ObservabilityService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
