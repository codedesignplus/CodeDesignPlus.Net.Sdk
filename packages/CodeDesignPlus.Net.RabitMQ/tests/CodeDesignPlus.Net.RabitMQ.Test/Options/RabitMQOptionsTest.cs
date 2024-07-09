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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void RabitMQOptions_UserNameIsRequired_FailedValidation(string? userName)
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Enable = true,
            Host = Guid.NewGuid().ToString(),
            UserName = userName,
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void RabitMQOptions_PortInvalid_FailedValidation(int port)
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Port = port,
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field Port must be between 1 and 65535.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6000)]
    public void RabitMQOptions_RetryIntervalInvalid_FailedValidation(int retryInterval)
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Port = 5672,
            RetryInterval = retryInterval,
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field RetryInterval must be between 1000 and 5000.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    public void RabitMQOptions_MaxRetryInvalid_FailedValidation(int maxRetry)
    {
        // Arrange
        var options = new RabitMQOptions()
        {
            Host = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            Port = 5672,
            MaxRetry = maxRetry,
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field MaxRetry must be between 1 and 10.");
    }
}
