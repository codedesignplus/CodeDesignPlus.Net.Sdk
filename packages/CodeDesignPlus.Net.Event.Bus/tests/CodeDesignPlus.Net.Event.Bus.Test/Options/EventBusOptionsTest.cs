namespace CodeDesignPlus.Net.Event.Bus.Test.Options;

public class EventBusOptionsTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EventBusOptions_DefaultValues_Valid(bool value)
    {
        // Arrange
        var options = new EventBusOptions()
        {
            EnableQueue = value
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal(value, options.EnableQueue);
    }
}
