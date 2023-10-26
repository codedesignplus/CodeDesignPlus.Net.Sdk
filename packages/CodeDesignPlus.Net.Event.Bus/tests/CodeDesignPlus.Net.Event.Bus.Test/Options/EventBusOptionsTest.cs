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
        Assert.Equal((uint)2, options.SecondsWaitQueue);
    }

    [Fact]
    public void EventBusOptions_Seconds_Valid()
    {
        // Arrange
        var secondsExpected = (uint)new Random().Next(1, 5);
        var options = new EventBusOptions()
        {
            EnableQueue = true,
            SecondsWaitQueue = secondsExpected
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal(secondsExpected, options.SecondsWaitQueue);
    }

    
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void EventBusOptions_Seconds_Invalid(uint seconds)
    {
        // Arrange
        var options = new EventBusOptions()
        {
            EnableQueue = true,
            SecondsWaitQueue = seconds
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
    }
}
