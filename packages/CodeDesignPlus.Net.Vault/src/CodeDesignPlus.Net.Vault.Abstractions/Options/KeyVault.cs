namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

/// <summary>
/// Options to configure the KeyVault service
/// </summary>
public class KeyVault
{
    /// <summary>
    /// Gets or sets the name of the key vault.
    /// </summary>
    public bool Enable { get; set; } = true;
    /// <summary>
    /// Gets or sets the name of the key vault.
    /// </summary>
    public string SufixMoundPoint { get; set; } = "keyvault";
}
