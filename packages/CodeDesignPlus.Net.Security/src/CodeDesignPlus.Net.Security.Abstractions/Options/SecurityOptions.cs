﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

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
    /// Gets or sets the audience to use in the authentication
    /// </summary>
    public bool IncludeErrorDetails { get; set; }
    /// <summary>
    /// Gets or sets the audience to use in the authentication
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether tokens must have an 'aud' claim matching the audience in the options.
    /// </summary>
    public bool RequireSignedTokens { get; set; }
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
    /// Gets or sets a value indicating whether the token signature will be validated during token validation.
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; }
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
    /// Gets or sets the certificate to use in the authentication
    /// </summary>
    public X509Certificate2 Certificate { get; set; }
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
}
