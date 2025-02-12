using System;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using Xunit;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class NeighborhoodTest
{
    [Fact]
    public void Neighborhood_Id_ShouldBeOfTypeGuid()
    {
        // Arrange
        var neighborhood = new Neighborhood();

        // Act
        var id = neighborhood.Id;

        // Assert
        Assert.IsType<Guid>(id);
    }

    [Fact]
    public void Neighborhood_Name_ShouldBeOfTypeString()
    {
        // Arrange
        var neighborhood = new Neighborhood();

        // Act
        var name = neighborhood.Name;

        // Assert
        Assert.IsType<string>(name);
    }

    [Fact]
    public void Neighborhood_Id_ShouldBeSetAndRetrievedCorrectly()
    {
        // Arrange
        var neighborhood = new Neighborhood();
        var expectedId = Guid.NewGuid();

        // Act
        neighborhood.Id = expectedId;
        var actualId = neighborhood.Id;

        // Assert
        Assert.Equal(expectedId, actualId);
    }

    [Fact]
    public void Neighborhood_Name_ShouldBeSetAndRetrievedCorrectly()
    {
        // Arrange
        var neighborhood = new Neighborhood();
        var expectedName = "Test Neighborhood";

        // Act
        neighborhood.Name = expectedName;
        var actualName = neighborhood.Name;

        // Assert
        Assert.Equal(expectedName, actualName);
    }
}
