namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to configure the RabbitMQ service
/// </summary>
public class RabbitMQ
{
    /// <summary>
    /// Gets or sets a value indicating whether the RabbitMQ service is enabled.
    /// </summary>
    public bool Enable { get; set; } = true;
    /// <summary>
    /// Gets or sets the sufix to mount the RabbitMQ service.
    /// </summary>
    [Required]
    public string RoleSufix { get; set; } = "rabbitmq-role";
    /// <summary>
    /// Gets or sets the sufix to mount the RabbitMQ service.
    /// </summary>
    [Required]
    public string SufixMoundPoint { get; set; } = "rabbitmq";
}
