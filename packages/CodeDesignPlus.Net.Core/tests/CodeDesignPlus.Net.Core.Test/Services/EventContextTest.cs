using CodeDesignPlus.Net.Core.Services;
using CodeDesignPlus.Net.Core.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.Core.Test.Services;

public class EventContextTest
{
    [Fact]
    public void SetCurrentDomainEvent_SetsCurrentDomainEvent()
    {
        // Arrange
        var eventContext = new EventContext();
        var tenant = Guid.NewGuid();
        var domainEvent = new ClientWithTenantDomainEvent(Guid.NewGuid(), "Goku", tenant);

        // Act
        eventContext.SetCurrentDomainEvent(domainEvent);

        // Assert
        Assert.Equal(domainEvent, eventContext.CurrentDomainEvent);
        Assert.Equal(tenant, eventContext.Tenant);
    }

    [Fact]
    public void SetCurrentDomainEvent_ThrowsException_WhenCurrentDomainEventIsAlreadySet()
    {
        // Arrange
        var eventContext = new EventContext();
        var tenant = Guid.NewGuid();
        var domainEvent = new ClientWithTenantDomainEvent(Guid.NewGuid(), "Goku", tenant);
        eventContext.SetCurrentDomainEvent(domainEvent);

        // Act & Assert
        Assert.Throws<CoreException>(() => eventContext.SetCurrentDomainEvent(domainEvent));
    }

    [Fact]
    public void AddMetadata_AddsMetadataEntry()
    {
        // Arrange
        var eventContext = new EventContext();
        var key = "key";
        var value = "value";

        // Act
        eventContext.AddMetadata(key, value);

        // Assert
        Assert.Equal(value, eventContext.GetMetadata(key));
    }

    [Fact]
    public void GetMetadata_ReturnsMetadataValue()
    {
        // Arrange
        var eventContext = new EventContext();
        var key = "key";
        var value = "value";
        eventContext.AddMetadata(key, value);

        // Act
        var result = eventContext.GetMetadata(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void TryGetMetadata_ReturnsTrue_WhenKeyExists()
    {
        // Arrange
        var eventContext = new EventContext();
        var key = "key";
        var value = "value";
        eventContext.AddMetadata(key, value);

        // Act
        var result = eventContext.TryGetMetadata(key, out var metadataValue);

        // Assert
        Assert.True(result);
        Assert.Equal(value, metadataValue);
    }

    [Fact]
    public void TryGetMetadata_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var eventContext = new EventContext();
        var key = "key";

        // Act
        var result = eventContext.TryGetMetadata(key, out var metadataValue);

        // Assert
        Assert.False(result);
        Assert.Null(metadataValue);
    }

    [Fact]
    public void RemoveMetadata_RemovesMetadataEntry()
    {
        // Arrange
        var eventContext = new EventContext();
        var key = "key";
        var value = "value";
        eventContext.AddMetadata(key, value);

        // Act
        eventContext.RemoveMetadata(key);

        // Assert
        Assert.False(eventContext.TryGetMetadata(key, out _));
    }

    [Fact]
    public void ClearMetadata_ClearsAllMetadataEntries()
    {
        // Arrange
        var eventContext = new EventContext();
        eventContext.AddMetadata("key1", "value1");
        eventContext.AddMetadata("key2", "value2");

        // Act
        eventContext.ClearMetadata();

        // Assert
        Assert.False(eventContext.TryGetMetadata("key1", out _));
        Assert.False(eventContext.TryGetMetadata("key2", out _));
    }

    [Fact]
    public void Dispose_ClearsMetadataAndSetsDisposed()
    {
        // Arrange
        var eventContext = new EventContext();
        eventContext.AddMetadata("key", "value");

        // Act
        eventContext.Dispose();

        // Assert
        Assert.Throws<NullReferenceException>(() => eventContext.TryGetMetadata("key", out _));
    }
}
