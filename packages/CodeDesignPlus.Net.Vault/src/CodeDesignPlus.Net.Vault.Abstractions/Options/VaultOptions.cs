using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to setting of the Vault
/// </summary>
public class VaultOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Vault";

    /// <summary>
    /// Gets or sets the address of the Vault server
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// Gets or sets the role of the Vault server
    /// </summary>
    [Required]
    public string RoleId { get; set; }
    /// <summary>
    /// Gets or sets the secret of the Vault server
    /// </summary>
    [Required]
    public string SecretId { get; set; }
    /// <summary>
    /// Gets or sets the solution to get the secrets
    /// </summary>
    [Required]
    public string Solution { get; set; }
    /// <summary>
    /// Gets or sets the name of the application
    /// </summary>
    public string AppName { get; set; }

    public KeyVault KeyVault { get; set; } = new();
    public Mongo Mongo { get; set; } = new();
    public RabbitMQ RabbitMQ { get; set; } = new();
}
