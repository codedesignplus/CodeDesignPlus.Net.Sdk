namespace CodeDesignPlus.Net.Redis.PubSub.Test.Options;

public class RedisPubSubOptionsTest
{
    [Fact]
    public void RedisPubSubOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new RedisPubSubOptions()
        {

        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

}
