using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Abstractions.Models;

public class FileTest
{

    [Fact]
    public void Constructor_FileNameIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var fullName = string.Empty;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => new M.File(fullName));

        // Assert
        Assert.Equal("fullName", exception.ParamName);
    }

    [Fact]
    public void Constructor_Default_Values()
    {
        // Arrange
        var fullName = "test.txt";

        // Act
        var file = new M.File(fullName);
        var metadata = file.GetMetadata(new Uri("http://localhost"));
        var tags = file.GetTags(1);

        // Assert
        var mimeType = ApacheMime.ApacheMimes.FirstOrDefault(x => x.Extension.Contains(file.Extension))!.ToString();
        Assert.Equal(fullName, file.FullName);
        Assert.Equal("test", file.Name);
        Assert.Equal(".txt", file.Extension);
        Assert.Equal(1, file.Version.Major);
        Assert.Equal(0, file.Version.Minor);
        Assert.Equal(0, file.Version.Patch);
        Assert.Equal(0, file.Size);
        Assert.False(file.Renowned);
        Assert.Equal(ApacheMime.ApacheMimes.FirstOrDefault(x => x.Extension.Contains(file.Extension)), file.Mime);

        Assert.Contains(metadata, x => x.Key == "FullName" && x.Value == fullName);
        Assert.Contains(metadata, x => x.Key == "Name" && x.Value == "test");
        Assert.Contains(metadata, x => x.Key == "Extension" && x.Value == ".txt");
        Assert.Contains(metadata, x => x.Key == "Size" && x.Value == "0");
        Assert.Contains(metadata, x => x.Key == "Version" && x.Value == "1.0.0");
        Assert.Contains(metadata, x => x.Key == "Renowned" && x.Value == "False");
        Assert.Contains(metadata, x => x.Key == "Mime" && x.Value == mimeType);
        Assert.Contains(metadata, x => x.Key == "Uri" && x.Value == "http://localhost/");
        Assert.Contains(metadata, x => x.Key == "CreatedAt" && DateTime.Parse(x.Value) <= DateTime.UtcNow);

        Assert.Contains(tags, x => x.Key == "Name" && x.Value == "test");
        Assert.Contains(tags, x => x.Key == "Mime" && x.Value == mimeType);
        Assert.Contains(tags, x => x.Key == "Tenant" && x.Value == "1");
    }

    [Fact]
    public void GetMetadata_Throw_ArgumentNullException_When_Uri_Is_Null()
    {
        // Arrange
        var fullName = "test.txt";
        var file = new M.File(fullName);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => file.GetMetadata(null!));

        // Assert
        Assert.Equal("uri", exception.ParamName);
    }

    [Fact]
    public void GetTags_Throw_ArgumentNullException_When_Tenant_Is_Null()
    {
        // Arrange
        var fullName = "test.txt";
        var file = new M.File(fullName);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => file.GetTags<string>(null!));

        // Assert
        Assert.Equal("tenant", exception.ParamName);
    }
}
