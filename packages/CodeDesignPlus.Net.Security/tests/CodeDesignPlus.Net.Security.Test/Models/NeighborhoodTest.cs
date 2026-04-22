namespace CodeDesignPlus.Net.Security.Test.Models;

public class NeighborhoodTest
{
    [Fact]
    public void Neighborhood_Create_SetsIdAndName_Correctly()
    {
        var id = Guid.NewGuid();
        var name = "Neighborhood";
        var neighborhood = Neighborhood.Create(id, name);
        Assert.Equal(id, neighborhood.Id);
        Assert.Equal(name, neighborhood.Name);
    }
}
