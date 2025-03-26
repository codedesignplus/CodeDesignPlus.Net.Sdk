using CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class LocalityTest
{
    [Fact]
    public void Locality_Id_ShouldBeOfTypeGuid()
    {
        // Arrange
        var locality = new Locality();

        // Act
        locality.Id = Guid.NewGuid();

        // Assert
        Assert.IsType<Guid>(locality.Id);
    }

    [Fact]
    public void Locality_Name_ShouldBeOfTypeString()
    {
        // Arrange
        var locality = new Locality();

        // Act
        locality.Name = "Test Locality";

        // Assert
        Assert.IsType<string>(locality.Name);
    }

    [Fact]
    public void Locality_Id_ShouldGetAndSetId()
    {
        // Arrange
        var locality = new Locality();
        var id = Guid.NewGuid();

        // Act
        locality.Id = id;

        // Assert
        Assert.Equal(id, locality.Id);
    }

    [Fact]
    public void Locality_Name_ShouldGetAndSetName()
    {
        // Arrange
        var locality = new Locality();
        var name = "Test Locality";

        // Act
        locality.Name = name;

        // Assert
        Assert.Equal(name, locality.Name);
    }
}