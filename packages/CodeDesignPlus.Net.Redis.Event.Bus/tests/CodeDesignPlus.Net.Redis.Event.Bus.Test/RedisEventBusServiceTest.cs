using CodeDesignPlus.Net.Redis.Event.Bus.Options;
using CodeDesignPlus.Net.Redis.Event.Bus.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test;

public class RedisEventBusServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new RedisEventBusOptions());

        // Act
        var actual = await new RedisEventBusService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
