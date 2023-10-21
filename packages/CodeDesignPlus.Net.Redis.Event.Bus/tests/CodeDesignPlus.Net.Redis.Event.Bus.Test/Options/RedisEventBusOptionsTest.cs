namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Options;

public class RedisEventBusOptionsTest
{
    [Fact]
    public void RedisEventBusOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new RedisEventBusOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void RedisEventBusOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new RedisEventBusOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }
}
