using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.Core.Test.Options;

public class CoreOptionsTest
{
    [Fact]
    public void CoreOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new CoreOptions()
        {
            AppName = Guid.NewGuid().ToString(),
            Version = "v1"
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
        Assert.Contains(results, x => x.ErrorMessage == "The AppName field is required.");
    }
}
