namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to configure the Kubernetes service
/// </summary>
public class Kubernetes
{
    /// <summary>
    /// Gets or sets a value indicating whether the Kubernetes service is enabled.
    /// </summary>
    public bool Enable { get; set; } = false;
    /// <summary>
    /// Gets or sets the sufix to mount the Kubernetes service.
    /// </summary>
    public string SufixMoundPoint { get; set; } = "k8s";
    /// <summary>
    /// Gets or sets the sufix to mount the Kubernetes service.
    /// </summary>
    public string RoleSufix { get; set; } = "k8s-role";
}
