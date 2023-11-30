using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.File.Storage.Test;

public class FileStorageServiceTest
{
    [Fact]
    public async Task Echo_ReturnSameValue_Equals()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var logger = Mock.Of<ILogger<FileStorageService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions());

        // Act
        var actual = await new FileStorageService(logger, options).EchoAsync(expected);

        // Assert
        Assert.Equal(expected, actual);
    }
}
