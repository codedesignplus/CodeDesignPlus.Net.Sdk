using CodeDesignPlus.Net.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.PubSub.Test.Options;

public class PubSubOptionsTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PubSubOptions_DefaultValues_Valid(bool value)
    {
        // Arrange
        var options = new PubSubOptions()
        {
            UseQueue = value
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal(value, options.UseQueue);
        Assert.Equal((uint)2, options.SecondsWaitQueue);
    }

    [Fact]
    public void PubSubOptions_Seconds_Valid()
    {
        // Arrange
        var secondsExpected = (uint)new Random().Next(1, 5);
        var options = new PubSubOptions()
        {
            UseQueue = true,
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
    public void PubSubOptions_Seconds_Invalid(uint seconds)
    {
        // Arrange
        var options = new PubSubOptions()
        {
            UseQueue = true,
            SecondsWaitQueue = seconds
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
    }
}
