using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Abstractions.Models;

public class FileDetailTest
{
    [Fact]
    public void Constructor_Default_Values()
    {
        // Arrange
        var uri = new Uri("http://localhost");
        var target = "test";
        var file = "test.txt";
        var provider = TypeProviders.AzureBlobProvider;

        // Act
        var path = new M.FileDetail(uri, target, file, provider);

        // Assert
        Assert.Equal(uri, path.Uri);
        Assert.Equal(target, path.Target);
        Assert.Equal(file, path.File);
        Assert.Equal(provider.ToString(), path.Provider);
        Assert.Equal($"{uri}/{(int)provider}?file={file}&target={target}", path.UriDownload);
        Assert.Equal($"{uri}/{(int)provider}?viewInBrowser=true&file={file}&target={target}", path.UriViewInBrowser);
    }

    [Fact]
    public void Constructor_Throw_ArgumentNullException_When_Uri_Is_Null()
    {
        // Arrange
        var target = "test";
        var file = "test.txt";
        var provider = TypeProviders.AzureBlobProvider;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => new M.FileDetail(null!, target, file, provider));

        // Assert
        Assert.Equal("uri", exception.ParamName);
    }

    [Fact]
    public void Constructor_Throw_ArgumentNullException_When_File_Is_Null()
    {
        // Arrange
        var uri = new Uri("http://localhost");
        var target = "test";
        var provider = TypeProviders.AzureBlobProvider;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => new M.FileDetail(uri, target, null!, provider));

        // Assert
        Assert.Equal("file", exception.ParamName);
    }


    [Fact]
    public void Constructor_TypeProviderIsNone_ThrowArgumentException()
    {
        // Arrange
        var uri = new Uri("http://localhost");
        var target = "test";
        var file = "test.txt";
        var provider = TypeProviders.None;

        // Act
        var exception = Assert.Throws<ArgumentException>(() => new M.FileDetail(uri, target, file, provider));

        // Assert
        Assert.Equal("provider", exception.ParamName);
    }
}
