using System;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using Xunit;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class CurrencyTest
{
    [Fact]
    public void Currency_Id_Should_Be_Guid()
    {
        // Arrange
        var currency = new Currency();
        var expected = Guid.NewGuid();

        // Act
        currency.Id = expected;

        // Assert
        Assert.Equal(expected, currency.Id);
    }

    [Fact]
    public void Currency_Name_Should_Be_Set_And_Get()
    {
        // Arrange
        var currency = new Currency();
        var expected = "US Dollar";

        // Act
        currency.Name = expected;

        // Assert
        Assert.Equal(expected, currency.Name);
    }

    [Fact]
    public void Currency_Code_Should_Be_Set_And_Get()
    {
        // Arrange
        var currency = new Currency();
        var expected = "USD";

        // Act
        currency.Code = expected;

        // Assert
        Assert.Equal(expected, currency.Code);
    }

    [Fact]
    public void Currency_Symbol_Should_Be_Set_And_Get()
    {
        // Arrange
        var currency = new Currency();
        var expected = "$";

        // Act
        currency.Symbol = expected;

        // Assert
        Assert.Equal(expected, currency.Symbol);
    }
}
