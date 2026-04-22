namespace CodeDesignPlus.Net.Security.Test.Models;

public class CityTest
{
    [Fact]
    public void City_Create_SetsId_Correctly()
    {
        var id = Guid.NewGuid();
        var city = City.Create(id, "New York", null);
        Assert.Equal(id, city.Id);
    }

    [Fact]
    public void City_Create_SetsName_Correctly()
    {
        var city = City.Create(Guid.NewGuid(), "New York", null);
        Assert.Equal("New York", city.Name);
    }

    [Fact]
    public void City_Create_SetsTimezone_Correctly()
    {
        var city = City.Create(Guid.NewGuid(), "New York", "Eastern Standard Time");
        Assert.Equal("Eastern Standard Time", city.Timezone);
    }

    [Fact]
    public void City_Create_AllowsNullTimezone()
    {
        var city = City.Create(Guid.NewGuid(), "New York", null);
        Assert.Null(city.Timezone);
    }
}
