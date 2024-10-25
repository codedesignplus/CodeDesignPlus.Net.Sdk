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
    [Required]
    public Mongo Mongo { get; set; } = new();
    /// <summary>
    /// Gets or sets the rabbitmq settings
    /// </summary>
    [Required]
    public RabbitMQ RabbitMQ { get; set; } = new();
    /// <summary>
    /// Gets or sets the kubernetes settings
    /// </summary>
    [Required]
    public Kubernetes Kubernetes { get; set; } = new();
    /// <summary>
    /// Gets or sets the transit settings
    /// </summary>
    [Required]
    public Transit Transit { get; set; } = new();

    /// <summary>
    /// Validate the properties of the class
    /// </summary>
    /// <param name="validationContext">The context of the validation</param>
    /// <returns>Returns a collection with the validation errors</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (string.IsNullOrEmpty(this.RoleId) && string.IsNullOrEmpty(this.SecretId) && this.Kubernetes != null && !this.Kubernetes.Enable)
        {
            results.Add(new ValidationResult("The RoleId and SecretId is required."));
        }

        if (this.Mongo != null && this.Mongo.Enable)
        {
            var contextMongo = new ValidationContext(this.Mongo, serviceProvider: null, items: null);

            Validator.TryValidateObject(this.Mongo, contextMongo, results, validateAllProperties: true);
        }

        if (this.RabbitMQ != null && this.RabbitMQ.Enable)
        {
            var contextRabbitMQ = new ValidationContext(this.RabbitMQ, serviceProvider: null, items: null);

            Validator.TryValidateObject(this.RabbitMQ, contextRabbitMQ, results, validateAllProperties: true);
        }

        if (this.Kubernetes != null && this.Kubernetes.Enable)
        {
            var contextKubernetes = new ValidationContext(this.Kubernetes, serviceProvider: null, items: null);

            Validator.TryValidateObject(this.Kubernetes, contextKubernetes, results, validateAllProperties: true);
        }

        return results;
    }
}
