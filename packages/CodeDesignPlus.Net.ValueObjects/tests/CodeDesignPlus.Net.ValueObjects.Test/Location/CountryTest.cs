using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Financial;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class CountryTest
{
    private static readonly Currency Currency = Currency.Create(Guid.NewGuid(), "Colombian Peso", "COP", "$", 2, 170);

    [Fact]
    public void Create_ValidParameters_ReturnsCountry()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var country = Country.Create(id, "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency);

        // Assert
        Assert.Equal(id, country.Id);
        Assert.Equal("Colombia", country.Name);
        Assert.Equal("CO", country.Alpha2);
        Assert.Equal("COL", country.Alpha3);
        Assert.Equal(170, country.Code);
        Assert.Equal("+57", country.PhoneCode);
        Assert.Equal("America/Bogota", country.Timezone);
        Assert.Equal(Currency, country.Currency);
    }

    [Fact]
    public void Create_LowercaseAlphaCodes_AreNormalizedToUpperCase()
    {
        // Act
        var country = Country.Create(Guid.NewGuid(), "Colombia", "  co  ", "  col  ", 170, "+57", "America/Bogota", Currency);

        // Assert
        Assert.Equal("CO", country.Alpha2);
        Assert.Equal("COL", country.Alpha3);
    }

    [Fact]
    public void Create_EmptyId_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.Empty, "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "000", "Country ID cannot be empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_NullOrEmptyName_ThrowsCodeDesignPlusException(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), name!, "CO", "COL", 170, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "001", "Country name cannot be empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyAlpha2_ThrowsCodeDesignPlusException(string? alpha2)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", alpha2!, "COL", 170, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "002", "Country Alpha2 code cannot be empty.");
    }

    [Theory]
    [InlineData("C")]
    [InlineData("COL")]
    public void Create_Alpha2LengthIsNotTwo_ThrowsCodeDesignPlusException(string alpha2)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", alpha2, "COL", 170, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "003", "Country Alpha2 code length is invalid.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyAlpha3_ThrowsCodeDesignPlusException(string? alpha3)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", "CO", alpha3!, 170, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "004", "Country Alpha3 code cannot be empty.");
    }

    [Theory]
    [InlineData("CO")]
    [InlineData("COLO")]
    public void Create_Alpha3LengthIsNotThree_ThrowsCodeDesignPlusException(string alpha3)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", "CO", alpha3, 170, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "005", "Country Alpha3 code length is invalid.");
    }

    [Theory]
    [InlineData((ushort)0)]
    [InlineData((ushort)1000)]
    public void Create_NumericCodeOutOfRange_ThrowsCodeDesignPlusException(ushort code)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", code, "+57", "America/Bogota", Currency));

        AssertGuard(exception, "006", "Country numeric code is invalid.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_NullOrEmptyPhoneCode_ThrowsCodeDesignPlusException(string? phoneCode)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, phoneCode!, "America/Bogota", Currency));

        AssertGuard(exception, "007", "Country phone code cannot be empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_NullOrEmptyTimezone_ThrowsCodeDesignPlusException(string? timezone)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", timezone!, Currency));

        AssertGuard(exception, "008", "Country timezone cannot be empty.");
    }

    [Fact]
    public void Create_NullCurrency_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", "America/Bogota", null!));

        AssertGuard(exception, "009", "Country currency is required.");
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var first = Country.Create(id, "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency);
        var second = Country.Create(id, "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency);

        // Assert
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Act
        var first = Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency);
        var second = Country.Create(Guid.NewGuid(), "Mexico", "MX", "MEX", 484, "+52", "America/Mexico_City", Currency);

        // Assert
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equality_AgainstNull_IsFalse()
    {
        // Act
        var country = Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "+57", "America/Bogota", Currency);

        // Assert
        Assert.False(country.Equals(null));
        Assert.False(country == null);
    }

    private static void AssertGuard(CodeDesignPlusException exception, string code, string message)
    {
        Assert.Equal(code, exception.Code);
        Assert.Equal(message, exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }
}
