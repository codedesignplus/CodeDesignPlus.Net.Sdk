using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.EventStore.Test.Options;

public class EventStoreOptionsTest
{
    [Fact]
    public void EventStoreOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventStoreOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

}
