namespace CodeDesignPlus.Net.Redis.Diagnostics.Test.Options;

public class Redis.DiagnosticsOptionsTest
{
    [Fact]
    public void Redis.DiagnosticsOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new Redis.DiagnosticsOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Redis.DiagnosticsOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new Redis.DiagnosticsOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void Redis.DiagnosticsOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new Redis.DiagnosticsOptions()
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
    public void Redis.DiagnosticsOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new Redis.DiagnosticsOptions()
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
