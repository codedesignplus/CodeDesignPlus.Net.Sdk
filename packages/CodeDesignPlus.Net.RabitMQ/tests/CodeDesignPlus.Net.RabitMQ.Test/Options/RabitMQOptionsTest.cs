namespace CodeDesignPlus.Net.RabitMQ.Test.Options;

public class RabitMQOptionsTest
{
    [Fact]
    public void RabitMQOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Port = 5672,
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void RabitMQOptions_UserNameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Port = 5672,
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The UserName field is required.");
    }

    [Fact]
    public void RabitMQOptions_PasaswordIsRequired_FailedValidation()
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString(),
            Port = 5672,
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Password field is required.");
    }
}
