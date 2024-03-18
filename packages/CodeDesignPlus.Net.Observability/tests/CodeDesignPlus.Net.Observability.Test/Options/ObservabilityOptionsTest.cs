using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Observability.Test.Options;

public class ObservabilityOptionsTest
{
    [Fact]
    public void ObservabilityOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void ObservabilityOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new ObservabilityOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void ObservabilityOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new ObservabilityOptions()
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
    public void ObservabilityOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new ObservabilityOptions()
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
