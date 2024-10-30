namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Type of authentication to use in the Vault
/// </summary>
public enum TypeAuth
{
    /// <summary>
    /// None authentication
    /// </summary>
    None,
    /// <summary>
    /// Token authentication
    /// </summary>
    Token,
    /// <summary>
    /// AppRole authentication
    /// </summary>
    AppRole,
    /// <summary>
    /// Kubernetes authentication
    /// </summary>
    Kubernetes
}
