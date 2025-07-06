using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Resources;

public class ResourcesOptionsTest
{
    [Fact]
    public void Section_ShouldReturnResources()
    {
        // Act
        var section = ResourcesOptions.Section;

        // Assert
        Assert.Equal("Resources", section);
    }

    [Fact]
    public void Enable_Property_ShouldGetAndSetValue()
    {
        // Arrange
        var options = new ResourcesOptions
        {
            Enable = true
        };

        // Assert
        Assert.True(options.Enable);

        // Act
        options.Enable = false;

        // Assert
        Assert.False(options.Enable);
    }

    [Fact]
    public void Server_Property_ShouldGetAndSetValue()
    {
        // Arrange
        var options = new ResourcesOptions();
        var uri = new Uri("https://example.com");

        // Act
        options.Server = uri;

        // Assert
        Assert.Equal(uri, options.Server);
    }
}
