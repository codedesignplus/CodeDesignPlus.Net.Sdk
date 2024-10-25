using System;
using VaultSharp.V1.SecretsEngines.Transit;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

public class Transit
{

    public TransitKeyType KeyType { get; set; } = TransitKeyType.aes256_gcm96;

}
