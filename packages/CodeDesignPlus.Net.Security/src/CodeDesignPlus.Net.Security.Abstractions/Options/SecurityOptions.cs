namespace CodeDesignPlus.Net.Security.Abstractions.Options;

/// <summary>
/// Options to setting of the Security
/// </summary>
public class SecurityOptions
{
    /// <summary>
    /// Name of the sections used in the appsettings
    /// </summary>
    public static readonly string Section = "Security";

    /// <summary>
    /// Gets or sets the authority to use in the authentication
    /// </summary>
    public string Authority { get; set; }

    /// <summary>
    /// Gets or sets the client id to use in the authentication
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include error details in the authentication responses
    /// </summary>
    public bool IncludeErrorDetails { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTTPS metadata is required for the authentication
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token audience must be validated
    /// </summary>
    public bool ValidateAudience { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token issuer must be validated
    /// </summary>
    public bool ValidateIssuer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token lifetime must be validated
    /// </summary>
    public bool ValidateLifetime { get; set; }

    /// <summary>
    /// Gets or sets the valid issuer that will be used to check against the token's issuer
    /// </summary>
    [Required]
    public string ValidIssuer { get; set; }

    /// <summary>
    /// Gets or sets the valid audiences that will be used to check against the token's audience
    /// </summary>
    [Required]
    public IEnumerable<string> ValidAudiences { get; set; }

    /// <summary>
    /// Gets or sets the applications to use in the authentication
    /// </summary>
    public string[] Applications { get; set; } = [];

    /// <summary>
    /// Gets or sets the path of the certificate to use in the authentication
    /// </summary>
    public string CertificatePath { get; set; }

    /// <summary>
    /// Gets or sets the password of the certificate to use in the authentication
    /// </summary>
    public string CertificatePassword { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to enable the tenant context
    /// </summary>
    public bool EnableTenantContext { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether to validate the license
    /// </summary>
    public bool ValidateLicense { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to validate the RBAC
    /// </summary>
    public bool ValidateRbac { get; set; }
    /// <summary>
    /// Gets or sets the server to use in the RBAC
    /// </summary>
    public Uri ServerRbac { get; set; }
    /// <summary>
    /// Gets or sets the interval to refresh the RBAC
    /// </summary>
    [Range(5, 20)]
    public ushort RefreshRbacInterval { get; set; } = 10;

    /// <summary>
    /// Get the certificate to use in the authentication
    /// </summary>
    /// <returns>The certificate to use in the authentication</returns>
    /// <exception cref="FileNotFoundException">Thrown when the certificate file is not found at the specified path</exception>
    public X509Certificate2 GetCertificate()
    {
        if (string.IsNullOrEmpty(this.CertificatePath))
            return null;

        var path = Path.Combine(AppContext.BaseDirectory, this.CertificatePath);

        if (!File.Exists(path))
            throw new FileNotFoundException($"The certificate file not found in the path: {path}");

        return X509CertificateLoader.LoadPkcs12FromFile(path, this.CertificatePassword);
    }
}