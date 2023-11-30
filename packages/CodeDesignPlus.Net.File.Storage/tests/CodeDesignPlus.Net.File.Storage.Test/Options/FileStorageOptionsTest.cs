using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.File.Storage.Test.Options;

public class FileStorageOptionsTest
{
    [Fact]
    public void FileStorageOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new FileStorageOptions()
        {
            Name = Guid.NewGuid().ToString()
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void FileStorageOptions_NameIsRequired_FailedValidation()
    {
        // Arrange
        var options = new FileStorageOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    }

    [Fact]
    public void FileStorageOptions_EmailIsRequired_FailedValidation()
    {
        // Arrange
        var options = new FileStorageOptions()
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
    public void FileStorageOptions_EmailIsInvalid_FailedValidation()
    {
        // Arrange
        var options = new FileStorageOptions()
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
