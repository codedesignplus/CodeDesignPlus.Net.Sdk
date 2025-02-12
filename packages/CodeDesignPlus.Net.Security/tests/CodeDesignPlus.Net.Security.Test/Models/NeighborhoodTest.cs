using CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class NeighborhoodTest
{
    [Fact]
    public void Neighborhood_Id_ShouldBeOfTypeGuid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Neighborhood";

        // Act
        var neighborhood = new Neighborhood() {
            Id = id,
            Name = name
        };

        // Assert 
        Assert.Equal(id, neighborhood.Id);
        Assert.Equal(name, neighborhood.Name);
    }
}
