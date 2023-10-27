using CodeDesignPlus.Net.Event.Sourcing.Options;
using CodeDesignPlus.Net.Event.Sourcing.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class EventSourcingServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<EventSourcingService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new EventSourcingOptions());

        // Act
        var actual = await new EventSourcingService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
