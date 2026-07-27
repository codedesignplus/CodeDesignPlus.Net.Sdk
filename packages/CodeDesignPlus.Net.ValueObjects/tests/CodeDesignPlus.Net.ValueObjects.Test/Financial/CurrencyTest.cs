using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Test.Financial;

public class CurrencyTest
{
    private const string Name = "Colombian Peso";
    private const string Code = "COP";
    private const string Symbol = "$";

    [Fact]
    public void Create_ValidParameters_ReturnsCurrency()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var currency = Currency.Create(id, Name, Code, Symbol, 2, 170);

        // Assert
        Assert.Equal(id, currency.CurrencyId);
        Assert.Equal(Name, currency.Name);
        Assert.Equal(Code, currency.Code);
        Assert.Equal(Symbol, currency.Symbol);
        Assert.Equal(2, currency.DecimalDigits);
        Assert.Equal(170, currency.NumericCode);
    }

    [Fact]
    public void Create_LowercaseCode_IsNormalizedToUpperCase()
    {
        // Act
        var currency = Currency.Create(Guid.NewGuid(), Name, "  cop  ", Symbol, 2, 170);

        // Assert
        Assert.Equal("COP", currency.Code);
    }

    [Fact]
    public void Create_ZeroDecimalDigits_IsAllowed()
    {
        // Arrange: hay monedas sin decimales, como el yen japones.
        // Act
        var currency = Currency.Create(Guid.NewGuid(), "Japanese Yen", "JPY", "¥", 0, 392);

        // Assert
        Assert.Equal(0, currency.DecimalDigits);
    }

    [Fact]
    public void Create_EmptyId_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.Empty, Name, Code, Symbol, 2, 170));

        Assert.Equal("000", exception.Code);
        Assert.Equal("CurrencyId is empty", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_NullOrEmptyName_ThrowsCodeDesignPlusException(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.NewGuid(), name!, Code, Symbol, 2, 170));

        Assert.Equal("001", exception.Code);
        Assert.Equal("Name is required", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Fact]
    public void Create_EmptyCode_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.NewGuid(), Name, string.Empty, Symbol, 2, 170));

        Assert.Equal("002", exception.Code);
        Assert.Equal($"Code is required for {Name}-", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData("CO")]
    [InlineData("COPX")]
    public void Create_CodeLengthIsNotThree_ThrowsCodeDesignPlusException(string code)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.NewGuid(), Name, code, Symbol, 2, 170));

        Assert.Equal("003", exception.Code);
        Assert.Equal($"Code length is invalid for {Name}-{code}", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_NullOrEmptySymbol_ThrowsCodeDesignPlusException(string? symbol)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.NewGuid(), Name, Code, symbol!, 2, 170));

        Assert.Equal("004", exception.Code);
        Assert.Equal($"Symbol is required for {Name}-{Code}", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData((short)0)]
    [InlineData((short)1000)]
    public void Create_NumericCodeOutOfRange_ThrowsCodeDesignPlusException(short numericCode)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.NewGuid(), Name, Code, Symbol, 2, numericCode));

        Assert.Equal("005", exception.Code);
        Assert.Equal($"Numeric code is invalid for {Name}-{Code}", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Fact]
    public void Create_NegativeDecimalDigits_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => Currency.Create(Guid.NewGuid(), Name, Code, Symbol, -1, 170));

        Assert.Equal("006", exception.Code);
        Assert.Equal($"Decimal digits is invalid for {Name}-{Code}", exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Fact]
    public void Create_NullCode_ThrowsNullReferenceException()
    {
        // El constructor normaliza con `code.Trim()` antes de cualquier guard, asi que un codigo
        // nulo revienta con NullReferenceException en vez de CodeDesignPlusException. Es un defecto
        // conocido: este test fija el comportamiento actual para que el arreglo se note.
        Assert.Throws<NullReferenceException>(() => Currency.Create(Guid.NewGuid(), Name, null!, Symbol, 2, 170));
    }
}
