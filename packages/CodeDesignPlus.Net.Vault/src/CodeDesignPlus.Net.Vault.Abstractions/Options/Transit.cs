namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to configure the Transit service
/// </summary>
public class Transit
{
    /// <summary>
    /// Gets or sets a value indicating whether the Transit service is enabled.
    /// </summary>
    public TransitKeyType KeyType { get; set; } = TransitKeyType.aes256_gcm96;
}
