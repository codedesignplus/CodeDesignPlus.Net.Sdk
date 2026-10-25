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
        var mimeType = "text/plain";
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

    [Theory]
    [InlineData("recording.mp4", "video/mp4")]
    [InlineData("photo.jpg", "image/jpeg")]
    [InlineData("minutes.pdf", "application/pdf")]
    [InlineData("grabacion-sesion-1-parte-1.mp3", "audio/mpeg")]
    public void Mime_ExtensionListedExactly_ReturnsThatMimeType(string fullName, string expected)
    {
        // Arrange
        var file = new M.File(fullName);

        // Act
        var mime = file.Mime;

        // Assert
        Assert.Equal(expected, mime.MimeType);
    }

    /// <summary>
    /// A file whose extension no MIME type lists is binary data: its tags and metadata are built anyway, instead of failing
    /// with a <see cref="NullReferenceException"/>.
    /// </summary>
    [Fact]
    public void GetTags_ExtensionWithoutMimeType_UsesBinaryContentType()
    {
        // Arrange
        var file = new M.File("recording.acsraw");

        // Act
        var tags = file.GetTags(Guid.Empty);
        var metadata = file.GetMetadata(new Uri("http://localhost"));

        // Assert
        Assert.Null(file.Mime);
        Assert.Equal(("application/octet-stream", "application/octet-stream", "application/octet-stream"), (file.ContentType, tags["Mime"], metadata["Mime"]));
    }
    /// <summary>
    /// La etiqueta de un blob solo admite letras y dígitos ASCII, espacio y <c>+ - . / : = _</c>: el nombre entra sin tildes
    /// y con lo demás cambiado por <c>_</c> (Kappali pendings/526).
    /// </summary>
    [Theory]
    [InlineData("Cotización (1)", "Cotizacion _1_")]
    [InlineData("Señalización & más", "Senalizacion _ mas")]
    [InlineData("acta-2026_v1.final", "acta-2026_v1.final")]
    public void ToTagValue_NameOutsideTheTagAlphabet_IsFolded(string name, string expected)
    {
        // Act
        var value = M.File.ToTagValue(name);

        // Assert
        Assert.Equal(expected, value);
    }

    [Fact]
    public void ToTagValue_NameLongerThanATag_IsCut()
    {
        // Act
        var value = M.File.ToTagValue(new string('a', 300));

        // Assert
        Assert.Equal(M.File.MaxTagValueLength, value.Length);
    }

    /// <summary>
    /// Los metadatos viajan como cabeceras HTTP, que solo llevan ASCII: el nombre va codificado y se recupera tal cual.
    /// </summary>
    [Fact]
    public void GetMetadata_NameWithAccents_IsAsciiAndDecodesBack()
    {
        // Arrange
        var file = new M.File("Cotización (1).pdf");

        // Act
        var metadata = file.GetMetadata(new Uri("http://localhost"));

        // Assert
        Assert.Equal((true, "Cotización (1).pdf", "Cotización (1)"), (metadata.Values.All(x => x.All(char.IsAscii)), Uri.UnescapeDataString(metadata["FullName"]), Uri.UnescapeDataString(metadata["Name"])));
    }

    /// <summary>
    /// Un archivo sin extensión es binario, no el primer tipo del catálogo.
    /// </summary>
    [Fact]
    public void Mime_NameWithoutExtension_IsBinary()
    {
        // Arrange
        var file = new M.File("README");

        // Assert
        Assert.Equal((null, "application/octet-stream"), (file.Mime, file.ContentType));
    }
}
