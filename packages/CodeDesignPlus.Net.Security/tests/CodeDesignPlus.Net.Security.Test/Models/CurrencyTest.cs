namespace CodeDesignPlus.Net.Security.Test.Models;

public class CurrencyTest
{
    [Fact]
    public void Currency_Create_SetsCurrencyId_Correctly()
    {
        var id = Guid.NewGuid();
        var currency = Currency.Create(id, "US Dollar", "USD", "$", 2, 840);
        Assert.Equal(id, currency.CurrencyId);
    }

    [Fact]
    public void Currency_Create_SetsName_Correctly()
    {
        var currency = Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);
        Assert.Equal("US Dollar", currency.Name);
    }

    [Fact]
    public void Currency_Create_SetsCode_Correctly()
    {
        var currency = Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);
        Assert.Equal("USD", currency.Code);
    }

    [Fact]
    public void Currency_Create_SetsSymbol_Correctly()
    {
        var currency = Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);
        Assert.Equal("$", currency.Symbol);
    }
}
