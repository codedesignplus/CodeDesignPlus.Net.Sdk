using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Options;

public class EventSourcingOptionsTest
{
    [Fact]
    public void EventSourcingOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventSourcingOptions();
        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void EventSourcingOptions_CustomValues_Valid()
    {
        // Arrange
        var aggregate = "ag";
        var snapshot = "sp";

        var options = new EventSourcingOptions()
        {
            MainName = aggregate,
            SnapshotSuffix = snapshot
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
        Assert.Equal(aggregate, options.MainName);
        Assert.Equal(snapshot, options.SnapshotSuffix);
    }

}
