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
    [Required]
    public string SufixMoundPoint { get; set; } = "k8s";
    /// <summary>
    /// Gets or sets the sufix to mount the Kubernetes service.
    /// </summary>
    [Required]
    public string RoleSufix { get; set; } = "k8s-role";
    /// <summary>
    /// Gets or sets the path of the token of the kubernetes
    /// </summary>
    public string PathTokenKubernetes { get; set; } = "/var/run/secrets/kubernetes.io/serviceaccount/token";
}
