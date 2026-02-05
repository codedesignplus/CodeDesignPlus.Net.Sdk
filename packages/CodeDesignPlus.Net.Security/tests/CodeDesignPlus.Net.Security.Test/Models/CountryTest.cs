using System;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using Xunit;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class CountryTest
{
    [Fact]
    public void Country_Id_Should_Be_Guid()
    {
        // Arrange
        var country = new Country();

        // Act
        country.Id = Guid.NewGuid();

        // Assert
        Assert.IsType<Guid>(country.Id);
    }

    [Fact]
    public void Country_Name_Should_Be_String()
    {
        // Arrange
        var country = new Country();

        // Act
        country.Name = "United States";

        // Assert
        Assert.IsType<string>(country.Name);
        Assert.Equal("United States", country.Name);
    }

    [Fact]
    public void Country_Code_Should_Be_String()
    {
        // Arrange
        var country = new Country();

        // Act
        country.Code = "US";

        // Assert
        Assert.IsType<string>(country.Code);
        Assert.Equal("US", country.Code);
    }

    [Fact]
    public void Country_TimeZone_Should_Be_String()
    {
        // Arrange
        var country = new Country();

        // Act
        country.TimeZone = "PST";

        // Assert
        Assert.IsType<string>(country.TimeZone);
        Assert.Equal("PST", country.TimeZone);
    }

    [Fact]
    public void Country_Currency_Should_Be_Currency()
    {
        // Arrange
        var country = new Country();
        var currency = new Currency { Code = 170, Symbol = "$" };

        // Act
        country.Currency = currency;

        // Assert
        Assert.IsType<Currency>(country.Currency);
        Assert.Equal(170, country.Currency.Code);
        Assert.Equal("$", country.Currency.Symbol);
    }
}
