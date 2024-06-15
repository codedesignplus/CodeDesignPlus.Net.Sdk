namespace CodeDesignPlus.Net.RabitMQ.Test.Options;

public class RabitMQOptionsTest
{
    [Fact]
    public void RabitMQOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void RabitMQOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new RabitMQOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void RabitMQOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Enable = true,
            Host = Guid.NewGuid().ToString(),
            UserName = null
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is required.");
    }

    [Fact]
    public void RabitMQOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Enable = true,
            Host = Guid.NewGuid().ToString(),
            UserName = "asdfasdfsdfgs"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    }
}
