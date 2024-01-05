using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Abstractions.Models;

public class ResponseTest
{
    [Fact]
    public void Create_Response_Success()
    {
        // Arrange
        var file = new M.File("test.txt");
        var typeProvider = TypeProviders.AzureBlobProvider;

        // Act
        var response = new M.Response(file, typeProvider);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(file, response.File);
        Assert.Equal(typeProvider.ToString(), response.Provider);
        Assert.False(response.Success);
        Assert.Null(response.Exception);
        Assert.Null(response.Message);
        Assert.Null(response.Stream);
    }

    [Fact]
    public void Create_Response_Throw_ArgumentNullException_When_File_Is_Null()
    {
        // Arrange
        var typeProvider = TypeProviders.AzureBlobProvider;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => new M.Response(null!, typeProvider));

        // Assert
        Assert.Equal("file", exception.ParamName);
    }

    [Fact]
    public void Create_TypeProviderIsNone_ThrowArgumentException()
    {
        // Arrange
        var file = new M.File("test.txt");
        var typeProvider = TypeProviders.None;

        // Act
        var exception = Assert.Throws<ArgumentException>(() => new M.Response(file, typeProvider));

        // Assert
        Assert.Equal("The type provider is not valid (Parameter 'typeProvider')", exception.Message);
        Assert.Equal("typeProvider", exception.ParamName);
    }
}
