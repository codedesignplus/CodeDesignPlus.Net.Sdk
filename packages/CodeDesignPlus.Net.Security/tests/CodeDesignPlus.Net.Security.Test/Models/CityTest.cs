using System;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using Xunit;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class CityTest
{
    [Fact]
    public void City_Id_Should_Be_Guid()
    {
        // Arrange
        var city = new City();
        var expected = Guid.NewGuid();

        // Act
        city.Id = expected;

        // Assert
        Assert.Equal(expected, city.Id);
    }

    [Fact]
    public void City_Name_Should_Be_String()
    {
        // Arrange
        var city = new City();
        var expected = "New York";

        // Act
        city.Name = expected;

        // Assert
        Assert.Equal(expected, city.Name);
    }

    [Fact]
    public void City_TimeZone_Should_Be_String()
    {
        // Arrange
        var city = new City();
        var expected = "Eastern Standard Time";

        // Act
        city.TimeZone = expected;

        // Assert
        Assert.Equal(expected, city.TimeZone);
    }
}
