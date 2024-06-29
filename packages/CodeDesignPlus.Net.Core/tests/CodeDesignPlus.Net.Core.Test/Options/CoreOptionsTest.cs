namespace CodeDesignPlus.Net.Core.Test.Options;

public class CoreOptionsTest
{
    [Fact]
    public void CoreOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Business = Guid.NewGuid().ToString(),
            AppName = Guid.NewGuid().ToString(),
            Version = "v1",
            Description = Guid.NewGuid().ToString(),
            Contact = new Contact()
            {
                Name = Guid.NewGuid().ToString(),
                Email = "codedesignplus@outlook.com"
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void CoreOptions_PropertiesIsRequired_FailedValidation()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Business = null!,
            AppName = null!,
            Version = null!,
            Description = null!,
            Contact = null!
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Business field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The AppName field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Version field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Description field is required.");
        Assert.Contains(results, x => x.ErrorMessage == "The Contact field is required.");
    }

    
    [Fact]
    public void CoreOptions_VersionRegex_FailedValidation()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Business = Guid.NewGuid().ToString(),
            AppName = Guid.NewGuid().ToString(),
            Version = "v1.0",
            Description = Guid.NewGuid().ToString(),
            Contact = new Contact()
            {
                Name = Guid.NewGuid().ToString(),
                Email = "codedesignplus@outlook.com"
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field Version must match the regular expression '^v\\d+$'.");
    }

    [Fact]
    public void CoreOptions_EmailInvalid_FailedValidation()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Business = Guid.NewGuid().ToString(),
            AppName = Guid.NewGuid().ToString(),
            Version = "v1",
            Description = Guid.NewGuid().ToString(),
            Contact = new Contact()
            {
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString()
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    }
}
