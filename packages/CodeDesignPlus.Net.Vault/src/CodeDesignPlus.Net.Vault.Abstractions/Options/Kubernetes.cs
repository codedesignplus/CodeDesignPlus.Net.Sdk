using System;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

public class Kubernetes
{
    public bool Enable { get; set; } = true;
    public string SufixMoundPoint { get; set; } = "k8s";
    public string RoleSufix { get; set; } = "k8s-role";
}
