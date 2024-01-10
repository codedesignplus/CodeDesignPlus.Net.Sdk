using CodeDesignPlus.Net.Security.Abstractions.Options;
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
            RequireSignedTokens = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            ValidIssuer = "https://localhost:5001",
            ValidAudiences = new string[] { "CodeDesignPlus.Net.Security.Test" }
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
        Assert.False(options.RequireSignedTokens);
        Assert.False(options.ValidateAudience);
        Assert.False(options.ValidateIssuer);
        Assert.False(options.ValidateLifetime);
        Assert.False(options.ValidateIssuerSigningKey);
        Assert.Equal("https://localhost:5001", options.ValidIssuer);
        Assert.Contains("CodeDesignPlus.Net.Security.Test", options.ValidAudiences);
    }
}
