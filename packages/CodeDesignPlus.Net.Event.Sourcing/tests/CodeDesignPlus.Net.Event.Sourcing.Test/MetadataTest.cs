namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class MetadataTest
{
    [Fact]
    public void Metadata_Constructor_SetsProperties()
    {
        // Arrange
        var expectedAggregateId = Guid.NewGuid();
        var expectedVersion = 1;
        var expectedUserId = Guid.NewGuid();
        var expectedCategory = "TestCategory";

        // Act
        var metadata = new Metadata(expectedAggregateId, expectedVersion, expectedUserId, expectedCategory);

        // Assert
        Assert.Equal(expectedAggregateId, metadata.AggregateId);
        Assert.Equal(expectedVersion, metadata.Version);
        Assert.Equal(expectedUserId, metadata.UserId);
        Assert.Equal(expectedCategory, metadata.Category);
    }
}
