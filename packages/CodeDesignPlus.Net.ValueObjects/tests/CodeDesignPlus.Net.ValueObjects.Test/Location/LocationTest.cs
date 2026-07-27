using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Financial;
using L = CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class LocationTest
{
    private const string Address = "Calle 10 #40-20";
    private const string PostalCode = "050021";

    private static readonly Currency Currency = Currency.Create(Guid.NewGuid(), "Colombian Peso", "COP", "$", 2, 170);
    private static readonly L.Country Country = L.Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency);
    private static readonly L.State State = L.State.Create(Guid.NewGuid(), "Antioquia", "ANT");
    private static readonly L.City City = L.City.Create(Guid.NewGuid(), "Medellin", "America/Bogota");
    private static readonly L.Locality Locality = L.Locality.Create(Guid.NewGuid(), "El Poblado");
    private static readonly L.Neighborhood Neighborhood = L.Neighborhood.Create(Guid.NewGuid(), "Provenza");

    [Fact]
    public void Create_ValidParameters_ReturnsLocation()
    {
        // Act
        var location = L.Location.Create(Country, State, City, Locality, Neighborhood, Address, PostalCode);

        // Assert
        Assert.Equal(Country, location.Country);
        Assert.Equal(State, location.State);
        Assert.Equal(City, location.City);
        Assert.Equal(Locality, location.Locality);
        Assert.Equal(Neighborhood, location.Neighborhood);
        Assert.Equal(Address, location.Address);
        Assert.Equal(PostalCode, location.PostalCode);
    }

    [Fact]
    public void Create_NullCountry_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(null!, State, City, Locality, Neighborhood, Address, PostalCode));

        AssertGuard(exception, "000", "Country cannot be null.");
    }

    [Fact]
    public void Create_NullState_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(Country, null!, City, Locality, Neighborhood, Address, PostalCode));

        AssertGuard(exception, "001", "State cannot be null.");
    }

    [Fact]
    public void Create_NullCity_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(Country, State, null!, Locality, Neighborhood, Address, PostalCode));

        AssertGuard(exception, "002", "City cannot be null.");
    }

    [Fact]
    public void Create_NullLocality_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(Country, State, City, null!, Neighborhood, Address, PostalCode));

        AssertGuard(exception, "003", "Locality cannot be null.");
    }

    [Fact]
    public void Create_NullNeighborhood_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(Country, State, City, Locality, null!, Address, PostalCode));

        AssertGuard(exception, "004", "Neighborhood cannot be null.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyAddress_ThrowsCodeDesignPlusException(string? address)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(Country, State, City, Locality, Neighborhood, address!, PostalCode));

        AssertGuard(exception, "005", "Address cannot be null or empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyPostalCode_ThrowsCodeDesignPlusException(string? postalCode)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => L.Location.Create(Country, State, City, Locality, Neighborhood, Address, postalCode!));

        AssertGuard(exception, "006", "Postal code cannot be null or empty.");
    }

    [Fact]
    public void Create_AddressAndPostalCodeWithSurroundingWhitespace_AreTrimmed()
    {
        // Act
        var location = L.Location.Create(Country, State, City, Locality, Neighborhood, $"  {Address}  ", $"  {PostalCode}  ");

        // Assert
        Assert.Equal(Address, location.Address);
        Assert.Equal(PostalCode, location.PostalCode);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Act
        var first = L.Location.Create(Country, State, City, Locality, Neighborhood, Address, PostalCode);
        var second = L.Location.Create(Country, State, City, Locality, Neighborhood, Address, PostalCode);

        // Assert
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentAddress_AreNotEqual()
    {
        // Act
        var first = L.Location.Create(Country, State, City, Locality, Neighborhood, Address, PostalCode);
        var second = L.Location.Create(Country, State, City, Locality, Neighborhood, "Carrera 43A #1-50", PostalCode);

        // Assert
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equality_AgainstNull_IsFalse()
    {
        // Act
        var location = L.Location.Create(Country, State, City, Locality, Neighborhood, Address, PostalCode);

        // Assert
        Assert.False(location.Equals(null));
        Assert.False(location == null);
    }

    private static void AssertGuard(CodeDesignPlusException exception, string code, string message)
    {
        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }
}
