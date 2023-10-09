namespace CodeDesignPlus.Net.Security.Test.Options;

public class SecurityOptionsTest
{
    [Fact]
    public void SecurityOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new SecurityOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void SecurityOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new SecurityOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void SecurityOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new SecurityOptions()
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
    public void SecurityOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new SecurityOptions()
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
