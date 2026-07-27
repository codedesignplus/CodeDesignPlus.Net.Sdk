using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class NeighborhoodTest
{
    [Fact]
    public void Create_ValidParameters_ReturnsNeighborhood()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var neighborhood = Neighborhood.Create(id, "Provenza");

        // Assert
        Assert.Equal(id, neighborhood.Id);
        Assert.Equal("Provenza", neighborhood.Name);
    }

    [Fact]
    public void Create_EmptyId_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Neighborhood.Create(Guid.Empty, "Provenza"));

        Assert.Equal("001", exception.Code);
        Assert.Equal("Neighborhood ID cannot be empty.", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyName_ThrowsCodeDesignPlusException(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Neighborhood.Create(Guid.NewGuid(), name!));

        Assert.Equal("002", exception.Code);
        Assert.Equal("Neighborhood name cannot be null or empty.", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_IsTrimmed()
    {
        // Act
        var neighborhood = Neighborhood.Create(Guid.NewGuid(), "  Provenza  ");

        // Assert
        Assert.Equal("Provenza", neighborhood.Name);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var first = Neighborhood.Create(id, "Provenza");
        var second = Neighborhood.Create(id, "Provenza");

        // Assert
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Act
        var first = Neighborhood.Create(Guid.NewGuid(), "Provenza");
        var second = Neighborhood.Create(Guid.NewGuid(), "Manila");

        // Assert
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equality_AgainstNull_IsFalse()
    {
        // Act
        var neighborhood = Neighborhood.Create(Guid.NewGuid(), "Provenza");

        // Assert
        Assert.False(neighborhood.Equals(null));
        Assert.False(neighborhood == null);
    }
}
