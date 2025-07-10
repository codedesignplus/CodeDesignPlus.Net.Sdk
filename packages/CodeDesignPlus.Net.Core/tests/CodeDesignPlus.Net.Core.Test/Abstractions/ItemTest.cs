namespace CodeDesignPlus.Net.Core.Test.Abstractions;

public class ItemTest
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = "TestValue";

        // Act
        var item = new Item<string>(id, value);

        // Assert
        Assert.Equal(id, item.Id);
        Assert.Equal(value, item.Value);
    }

    [Fact]
    public void Records_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = 42;

        // Act
        var item1 = new Item<int>(id, value);
        var item2 = new Item<int>(id, value);

        // Assert
        Assert.Equal(item1, item2);
        Assert.True(item1 == item2);
    }

    [Fact]
    public void Records_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var item1 = new Item<int>(id, 1);
        var item2 = new Item<int>(id, 2);

        // Assert
        Assert.NotEqual(item1, item2);
        Assert.False(item1 == item2);
    }
}
