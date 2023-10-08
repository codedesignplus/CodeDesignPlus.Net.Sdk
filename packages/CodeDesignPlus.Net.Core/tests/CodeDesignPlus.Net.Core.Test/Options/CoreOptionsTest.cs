namespace CodeDesignPlus.Net.Core.Test.Options;

public class CoreOptionsTest
{
    [Fact]
    public void CoreOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void CoreOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new CoreOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void CoreOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Enable = true,
            Name = Guid.NewGuid().ToString(),
            Email = null
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is required.");
    }

    [Fact]
    public void CoreOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new CoreOptions()
        {
            Enable = true,
            Name = Guid.NewGuid().ToString(),
            Email = "asdfasdfsdfgs"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    }
}
