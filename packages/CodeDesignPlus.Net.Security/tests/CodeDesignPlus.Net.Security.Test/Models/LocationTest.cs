namespace CodeDesignPlus.Net.Security.Test.Models;

public class LocationTest
{
    private static Currency CreateCurrency() =>
        Currency.Create(Guid.NewGuid(), "US Dollar", "USD", "$", 2, 840);

    private static Country CreateCountry() =>
        Country.Create(Guid.NewGuid(), "United States", "US", "USA", 840, "PST", CreateCurrency());

    private static State CreateState() =>
        State.Create(Guid.NewGuid(), "California", "CA");

    private static City CreateCity() =>
        City.Create(Guid.NewGuid(), "New York", null);

    private static Locality CreateLocality() =>
        Locality.Create(Guid.NewGuid(), "Test Locality");

    private static Neighborhood CreateNeighborhood() =>
        Neighborhood.Create(Guid.NewGuid(), "Test Neighborhood");

    [Fact]
    public void Location_Create_SetsCountry_Correctly()
    {
        var country = CreateCountry();
        var location = Location.Create(country, CreateState(), CreateCity(), CreateLocality(), CreateNeighborhood(), "123 Main St", "12345");
        Assert.Equal(country, location.Country);
    }

    [Fact]
    public void Location_Create_SetsState_Correctly()
    {
        var state = CreateState();
        var location = Location.Create(CreateCountry(), state, CreateCity(), CreateLocality(), CreateNeighborhood(), "123 Main St", "12345");
        Assert.Equal(state, location.State);
    }

    [Fact]
    public void Location_Create_SetsCity_Correctly()
    {
        var city = CreateCity();
        var location = Location.Create(CreateCountry(), CreateState(), city, CreateLocality(), CreateNeighborhood(), "123 Main St", "12345");
        Assert.Equal(city, location.City);
    }

    [Fact]
    public void Location_Create_SetsLocality_Correctly()
    {
        var locality = CreateLocality();
        var location = Location.Create(CreateCountry(), CreateState(), CreateCity(), locality, CreateNeighborhood(), "123 Main St", "12345");
        Assert.Equal(locality, location.Locality);
    }

    [Fact]
    public void Location_Create_SetsNeighborhood_Correctly()
    {
        var neighborhood = CreateNeighborhood();
        var location = Location.Create(CreateCountry(), CreateState(), CreateCity(), CreateLocality(), neighborhood, "123 Main St", "12345");
        Assert.Equal(neighborhood, location.Neighborhood);
    }
}
