using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to setting of the Vault
/// </summary>
public class VaultOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Vault";

    /// <summary>
    /// Gets or sets the address of the Vault server
    /// </summary>
    [Required]
    [Url]
    public string Address { get; set; }
    /// <summary>
    /// Gets or sets the role of the Vault server
    /// </summary>
    public string RoleId { get; set; }
    /// <summary>
    /// Gets or sets the secret of the Vault server
    /// </summary>
    public string SecretId { get; set; }
    /// <summary>
    /// Gets or sets the solution to get the secrets
    /// </summary>
    [Required]
    public string Solution { get; set; }
    /// <summary>
    /// Gets or sets the name of the application
    /// </summary>
    [Required]
    public string AppName { get; set; }
    /// <summary>
    /// Gets or sets the keyvault settings
    /// </summary>
    public KeyVault KeyVault { get; set; } = new();
    /// <summary>
    /// Gets or sets the mongo settings
    /// </summary>
    public Mongo Mongo { get; set; } = new();
    /// <summary>
    /// Gets or sets the rabbitmq settings
    /// </summary>
    public RabbitMQ RabbitMQ { get; set; } = new();
    /// <summary>
    /// Gets or sets the kubernetes settings
    /// </summary>
    public Kubernetes Kubernetes { get; set; } = new();
    /// <summary>
    /// Gets or sets the transit settings
    /// </summary>
    public Transit Transit { get; set; } = new();

    /// <summary>
    /// Validate the properties of the class
    /// </summary>
    /// <param name="validationContext">The context of the validation</param>
    /// <returns>Returns a collection with the validation errors</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(this.RoleId) && string.IsNullOrEmpty(this.SecretId) && this.Kubernetes.Enable)
        {
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                yield return new ValidationResult("The RoleId or SecretId is required.");
            }
        }
    }
}
