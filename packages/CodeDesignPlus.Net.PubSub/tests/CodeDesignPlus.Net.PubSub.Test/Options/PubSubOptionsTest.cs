namespace CodeDesignPlus.Net.PubSub.Test.Options;

public class PubSubOptionsTest
{
    [Fact]
    public void PubSubOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new PubSubOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void PubSubOptions_DefaultValues_OutboxEnabledAndSentKeptSevenDays()
    {
        // Arrange
        var options = new PubSubOptions();

        // Assert
        Assert.Equal((true, 7u, 30u), (options.Outbox.Enable, options.Outbox.SentRetentionDays, options.PublishTimeoutSeconds));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(301)]
    public void PubSubOptions_PublishTimeout_Invalid(uint seconds)
    {
        // Arrange
        var options = new PubSubOptions()
        {
            PublishTimeoutSeconds = seconds
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
    }

    [Fact]
    public void OutboxOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new OutboxOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(366)]
    public void OutboxOptions_SentRetentionDays_Invalid(uint days)
    {
        // Arrange
        var options = new OutboxOptions()
        {
            SentRetentionDays = days
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
    }
}
