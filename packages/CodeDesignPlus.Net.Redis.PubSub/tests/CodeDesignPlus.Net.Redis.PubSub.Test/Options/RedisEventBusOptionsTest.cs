namespace CodeDesignPlus.Net.Redis.PubSub.Test.Options;

public class RedisPubSubOptionsTest
{
    [Fact]
    public void RedisPubSubOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new RedisPubSubOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void RedisPubSubOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new RedisPubSubOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }
}
