using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class CityTest
{
    [Fact]
    public void Create_ValidParameters_ReturnsCity()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var city = City.Create(id, "Medellin", "America/Bogota");

        // Assert
        Assert.Equal(id, city.Id);
        Assert.Equal("Medellin", city.Name);
        Assert.Equal("America/Bogota", city.Timezone);
    }

    [Fact]
    public void Create_EmptyId_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => City.Create(Guid.Empty, "Medellin", "America/Bogota"));

        Assert.Equal("001", exception.Code);
        Assert.Equal("City ID cannot be empty.", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyName_ThrowsCodeDesignPlusException(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => City.Create(Guid.NewGuid(), name!, "America/Bogota"));

        Assert.Equal("002", exception.Code);
        Assert.Equal("City name cannot be null or empty.", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithoutTimezone_IsAllowed(string? timezone)
    {
        // Arrange: la zona horaria de la ciudad es opcional; quien la consuma cae a la del pais.
        // Act
        var city = City.Create(Guid.NewGuid(), "Medellin", timezone);

        // Assert
        Assert.Equal(timezone, city.Timezone);
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_IsTrimmed()
    {
        // Act
        var city = City.Create(Guid.NewGuid(), "  Medellin  ", "America/Bogota");

        // Assert
        Assert.Equal("Medellin", city.Name);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var first = City.Create(id, "Medellin", "America/Bogota");
        var second = City.Create(id, "Medellin", "America/Bogota");

        // Assert
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Act
        var first = City.Create(Guid.NewGuid(), "Medellin", "America/Bogota");
        var second = City.Create(Guid.NewGuid(), "Bogota", "America/Bogota");

        // Assert
        Assert.False(first == second);
        Assert.True(first != second);
        Assert.False(first.Equals((object)second));
    }

    [Fact]
    public void Equality_AgainstNull_IsFalse()
    {
        // Act
        var city = City.Create(Guid.NewGuid(), "Medellin", "America/Bogota");

        // Assert
        Assert.False(city.Equals(null));
        Assert.False(city == null);
        Assert.True(city != null);
    }
}
