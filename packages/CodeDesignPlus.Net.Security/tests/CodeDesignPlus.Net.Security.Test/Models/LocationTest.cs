using System;
using Xunit;
using CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class LocationTest
{
    [Fact]
    public void Location_CountryProperty_SetAndGet()
    {
        // Arrange
        var location = new Location();
        var country = new Country();

        // Act
        location.Country = country;

        // Assert
        Assert.Equal(country, location.Country);
    }

    [Fact]
    public void Location_StateProperty_SetAndGet()
    {
        // Arrange
        var location = new Location();
        var state = new State();

        // Act
        location.State = state;

        // Assert
        Assert.Equal(state, location.State);
    }

    [Fact]
    public void Location_CityProperty_SetAndGet()
    {
        // Arrange
        var location = new Location();
        var city = new City();

        // Act
        location.City = city;

        // Assert
        Assert.Equal(city, location.City);
    }

    [Fact]
    public void Location_LocalityProperty_SetAndGet()
    {
        // Arrange
        var location = new Location();
        var locality = new Locality();

        // Act
        location.Locality = locality;

        // Assert
        Assert.Equal(locality, location.Locality);
    }

    [Fact]
    public void Location_NeighborhoodProperty_SetAndGet()
    {
        // Arrange
        var location = new Location();
        var neighborhood = new Neighborhood();

        // Act
        location.Neighborhood = neighborhood;

        // Assert
        Assert.Equal(neighborhood, location.Neighborhood);
    }
}
