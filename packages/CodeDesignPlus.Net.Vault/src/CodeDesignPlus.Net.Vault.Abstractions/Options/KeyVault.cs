using System;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

public class KeyVault
{
    public bool Enable { get; set; } = true;
    public string SufixMoundPoint { get; set; } = "keyvault";
}
