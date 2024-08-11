using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Options;

public class EventStorePubSubOptionsTest
{
    [Fact]
    public void EventStorePubSubOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventStorePubSubOptions()
        {
            Group = "Group"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal("Group", options.Group);
    }

    [Fact]
    public void EventStorePubSubOptions_InvalidValues_Invalid()
    {
        // Arrange
        var options = new EventStorePubSubOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, item => item.ErrorMessage == "The Group field is required.");
    }
}
