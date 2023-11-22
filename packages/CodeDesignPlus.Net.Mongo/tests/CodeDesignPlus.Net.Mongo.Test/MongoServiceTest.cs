using CodeDesignPlus.Net.Mongo.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Test;

public class MongoServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<MongoService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new MongoOptions());

        // Act
        var actual = await new MongoService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
