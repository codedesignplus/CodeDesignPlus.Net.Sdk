namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to configure the Mongo service
/// </summary>
public class Mongo
{
    /// <summary>
    /// Gets or sets a value indicating whether the Mongo service is enabled.
    /// </summary>
    public bool Enable { get; set; } = true;
    /// <summary>
    /// Gets or sets the sufix to mount the Mongo service.
    /// </summary>
    [Required]
    public string RoleSufix { get; set; } = "mongo-role";
    /// <summary>
    /// Gets or sets the sufix to mount the Mongo service.
    /// </summary>
    [Required]
    public string SufixMoundPoint { get; set; } = "database";
    /// <summary>
    /// Gets or sets the connection string template to connect to the Mongo service.
    /// </summary>
    [Required]
    public string TemplateConnectionString { get; set; } = "mongodb://{0}:{1}@mongo:27017";
}
