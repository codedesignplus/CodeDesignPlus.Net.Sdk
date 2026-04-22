namespace CodeDesignPlus.Net.Security.Test.Models;

public class LocalityTest
{
    [Fact]
    public void Locality_Create_SetsId_Correctly()
    {
        var id = Guid.NewGuid();
        var locality = Locality.Create(id, "Test Locality");
        Assert.IsType<Guid>(locality.Id);
        Assert.Equal(id, locality.Id);
    }

    [Fact]
    public void Locality_Create_SetsName_Correctly()
    {
        var locality = Locality.Create(Guid.NewGuid(), "Test Locality");
        Assert.IsType<string>(locality.Name);
        Assert.Equal("Test Locality", locality.Name);
    }
}
