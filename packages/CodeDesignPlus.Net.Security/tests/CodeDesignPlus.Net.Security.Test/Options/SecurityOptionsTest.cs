using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Security.Test.Options;

public class SecurityOptionsTest
{

    [Fact]
    public void Security_GetAndSet_Success()
    {
        // Arrange
        var options = new SecurityOptions()
        {
            Authority = "https://localhost:5001",
            Applications = ["CodeDesignPlus.Net.Security.Test"],
            CertificatePath = "certificate.pfx",
            CertificatePassword = "123456",
            IncludeErrorDetails = true,
            RequireHttpsMetadata = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidIssuer = "https://localhost:5001",
            ValidAudiences = ["CodeDesignPlus.Net.Security.Test"]
        };

        // Act
        var result = options.Validate();

        // Assert
        Assert.Empty(result);
        Assert.Equal("https://localhost:5001", options.Authority);
        Assert.Contains("CodeDesignPlus.Net.Security.Test", options.Applications);
        Assert.Equal("certificate.pfx", options.CertificatePath);
        Assert.Equal("123456", options.CertificatePassword);
        Assert.True(options.IncludeErrorDetails);
        Assert.False(options.RequireHttpsMetadata);
        Assert.False(options.ValidateAudience);
        Assert.False(options.ValidateIssuer);
        Assert.False(options.ValidateLifetime);
        Assert.Equal("https://localhost:5001", options.ValidIssuer);
        Assert.Contains("CodeDesignPlus.Net.Security.Test", options.ValidAudiences);
    }

    [Fact]
    public void GetCertificate_NullPath_ReturnNull()
    {
        // Arrange
        var options = new SecurityOptions()
        {
            CertificatePath = null,
        };

        // Act
        var result = options.GetCertificate();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCertificate_FileNotExist_ThrowFileNotFoundException()
    {
        // Arrange
        var options = new SecurityOptions()
        {
            CertificatePath = "folder/certificate.pfx",
        };

        var path = Path.Combine(AppContext.BaseDirectory, options.CertificatePath);

        // Act
        var exception = Assert.Throws<FileNotFoundException>(() => options.GetCertificate());

        // Assert
        Assert.Equal($"The certificate file not found in the path: {path}", exception.Message);
    }

    [Fact]
    public void GetCertificate_Success()
    {
        // Arrange
        var options = new SecurityOptions()
        {
            CertificatePath = "Helpers/Certificates/identity.pfx",
            CertificatePassword = "Temporal1"
        };

        // Act
        var result = options.GetCertificate();

        // Assert
        Assert.NotNull(result);
    }
}
