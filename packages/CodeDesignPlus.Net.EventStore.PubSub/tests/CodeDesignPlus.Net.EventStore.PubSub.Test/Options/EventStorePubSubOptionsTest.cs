using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Options;

public class EventStorePubSubOptionsTest
{
    [Fact]
    public void EventStorePubSubOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventStorePubSubOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
