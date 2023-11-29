namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class MetadataTest
{
    [Fact]
    public void Metadata_SetProperties_SetCorrectly()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var version = 1L;
        var userId = Guid.NewGuid();
        var category = "SampleCategory";

        // Act
        var metadata = new Metadata<Guid>(aggregateId, version, userId, category);

        // Assert
        Assert.Equal(aggregateId, metadata.AggregateId);
        Assert.Equal(version, metadata.Version);
        Assert.Equal(userId, metadata.UserId);
        Assert.Equal(category, metadata.Category);
        Assert.IsType<Guid>(metadata.AggregateId);
    }
}
