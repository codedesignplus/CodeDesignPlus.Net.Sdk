using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.File.Storage.Test.Options;

public class FileStorageOptionsTest
{
    [Fact]
    public void FileStorageOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new FileStorageOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

}
