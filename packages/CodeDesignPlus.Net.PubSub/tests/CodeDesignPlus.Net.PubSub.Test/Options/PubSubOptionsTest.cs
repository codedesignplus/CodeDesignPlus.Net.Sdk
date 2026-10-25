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

    /// <summary>
    /// Sin configurar, la copia repite a los 1, 5 y 15 minutos, y después cada hora (pendings/466).
    /// </summary>
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 5)]
    [InlineData(2, 15)]
    [InlineData(3, 60)]
    [InlineData(10, 60)]
    public void SnapshotRetryDelayOf_DefaultValues_GrowThenFollowTheInterval(int attempt, int minutes)
    {
        // Arrange
        var options = new PubSubOptions();

        // Act
        var delay = options.SnapshotRetryDelayOf(attempt);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(minutes), delay);
    }

    [Fact]
    public void SnapshotRetryDelayOf_IntervalDisabled_ReturnsZero()
    {
        // Arrange
        var options = new PubSubOptions { SnapshotRetryInterval = TimeSpan.Zero };

        // Act
        var delay = options.SnapshotRetryDelayOf(0);

        // Assert
        Assert.Equal(TimeSpan.Zero, delay);
    }

    [Fact]
    public void SnapshotRetryDelayOf_NonPositiveDelay_FallsBackToTheInterval()
    {
        // Arrange
        var options = new PubSubOptions { SnapshotRetryDelays = [TimeSpan.Zero, TimeSpan.FromMinutes(-1)] };

        // Act
        var delays = (options.SnapshotRetryDelayOf(0), options.SnapshotRetryDelayOf(1));

        // Assert
        Assert.Equal((TimeSpan.FromHours(1), TimeSpan.FromHours(1)), delays);
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
