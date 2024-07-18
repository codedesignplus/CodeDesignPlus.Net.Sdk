namespace CodeDesignPlus.Net.Security.Abstractions.Options;

/// <summary>
/// Options to setting of the Security
/// </summary>
public class SecurityOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
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
    /// Gets or sets the audience to use in the authentication
    /// </summary>
    public bool IncludeErrorDetails { get; set; }
    /// <summary>
    /// Gets or sets the audience to use in the authentication
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the token signature must be validated.
    /// </summary>
    public bool ValidateAudience { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the token lifetime will be validated.
    /// </summary>
    public bool ValidateIssuer { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the token issuer will be validated during token validation.
    /// </summary>
    public bool ValidateLifetime { get; set; }
    /// <summary>
    /// Gets or sets the valid issuer that will be used to check against the token's issuer.
    /// </summary>
    [Required]
    public string ValidIssuer { get; set; }
    /// <summary>
    /// Gets or sets the valid audiences that will be used to check against the token's audience.
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
    /// Get the certificate to use in the authentication
    /// </summary>
    /// <returns>The certificate to use in the authentication</returns>
    public X509Certificate2 GetCertificate()
    {
        if (string.IsNullOrEmpty(this.CertificatePath))
            return null;

        var path = Path.Combine(AppContext.BaseDirectory, this.CertificatePath);

        if (!File.Exists(path))
            throw new FileNotFoundException($"The certificate file not found in the path: {path}");

        return new X509Certificate2(this.CertificatePath, this.CertificatePassword);
    }
}
