namespace CodeDesignPlus.Net.Mongo.Diagnostics.Test.Options;

public class MongoDiagnosticsOptionsTest
{
    [Fact]
    public void MongoDiagnosticsOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new MongoDiagnosticsOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void MongoDiagnosticsOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new MongoDiagnosticsOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void MongoDiagnosticsOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new MongoDiagnosticsOptions()
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
    public void MongoDiagnosticsOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new MongoDiagnosticsOptions()
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
