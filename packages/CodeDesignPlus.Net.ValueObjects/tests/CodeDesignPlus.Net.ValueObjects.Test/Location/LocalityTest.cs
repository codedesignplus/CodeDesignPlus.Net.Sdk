using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class LocalityTest
{
    [Fact]
    public void Create_ValidParameters_ReturnsLocality()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var locality = Locality.Create(id, "El Poblado");

        // Assert
        Assert.Equal(id, locality.Id);
        Assert.Equal("El Poblado", locality.Name);
    }

    [Fact]
    public void Create_EmptyId_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Locality.Create(Guid.Empty, "El Poblado"));

        Assert.Equal("001", exception.Code);
        Assert.Equal("Locality ID cannot be empty.", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyName_ThrowsCodeDesignPlusException(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Locality.Create(Guid.NewGuid(), name!));

        Assert.Equal("002", exception.Code);
        Assert.Equal("Locality name cannot be null or empty.", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_IsTrimmed()
    {
        // Act
        var locality = Locality.Create(Guid.NewGuid(), "  El Poblado  ");

        // Assert
        Assert.Equal("El Poblado", locality.Name);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var first = Locality.Create(id, "El Poblado");
        var second = Locality.Create(id, "El Poblado");

        // Assert
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Act
        var first = Locality.Create(Guid.NewGuid(), "El Poblado");
        var second = Locality.Create(Guid.NewGuid(), "Laureles");

        // Assert
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equality_AgainstNull_IsFalse()
    {
        // Act
        var locality = Locality.Create(Guid.NewGuid(), "El Poblado");

        // Assert
        Assert.False(locality.Equals(null));
        Assert.False(locality == null);
    }
}
