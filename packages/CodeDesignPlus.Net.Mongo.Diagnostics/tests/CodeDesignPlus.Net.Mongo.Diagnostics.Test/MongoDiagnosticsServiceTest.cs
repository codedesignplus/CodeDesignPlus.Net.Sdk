using CodeDesignPlus.Net.Mongo.Diagnostics.Options;
using CodeDesignPlus.Net.Mongo.Diagnostics.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Diagnostics.Test;

public class MongoDiagnosticsServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<MongoDiagnosticsService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new MongoDiagnosticsOptions());

        // Act
        var actual = await new MongoDiagnosticsService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
