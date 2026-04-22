namespace CodeDesignPlus.Net.Security.Test.Models;

public class CountryTest
{
    private static Currency CreateCurrency() =>
        Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);

    [Fact]
    public void Country_Create_SetsId_Correctly()
    {
        var id = Guid.NewGuid();
        var country = Country.Create(id, "United States", "US", "USA", 840, "PST", CreateCurrency());
        Assert.IsType<Guid>(country.Id);
        Assert.Equal(id, country.Id);
    }

    [Fact]
    public void Country_Create_SetsName_Correctly()
    {
        var country = Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "PST", CreateCurrency());
        Assert.IsType<string>(country.Name);
        Assert.Equal("United States", country.Name);
    }

    [Fact]
    public void Country_Create_SetsAlpha2_Correctly()
    {
        var country = Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "PST", CreateCurrency());
        Assert.IsType<string>(country.Alpha2);
        Assert.Equal("US", country.Alpha2);
    }

    [Fact]
    public void Country_Create_SetsTimezone_Correctly()
    {
        var country = Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "PST", CreateCurrency());
        Assert.IsType<string>(country.Timezone);
        Assert.Equal("PST", country.Timezone);
    }

    [Fact]
    public void Country_Create_SetsCurrency_Correctly()
    {
        var currency = Currency.Create(Guid.NewGuid(), "Colombian Peso", "COP", "$", 2, 170);
        var country = Country.Create(Guid.NewGuid(), "Colombia", "CO", "COL", 170, "America/Bogota", currency);
        Assert.IsType<Currency>(country.Currency);
        Assert.Equal("COP", country.Currency.Code);
        Assert.Equal("$", country.Currency.Symbol);
    }
}
