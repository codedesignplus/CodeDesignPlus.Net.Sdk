namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// Proveedor thread-safe de <see cref="IVaultClient"/> que permite reemplazar la instancia
/// en caliente cuando se necesita re-autenticar (ej. token expirado o max_ttl alcanzado).
/// </summary>
/// <param name="options">Opciones de configuración de Vault usadas para crear el cliente inicial.</param>
public sealed class VaultClientProvider(VaultOptions options)
{
    private volatile IVaultClient client = VaultClientFactory.Create(options);

    /// <summary>
    /// Obtiene la instancia actual de <see cref="IVaultClient"/>.
    /// </summary>
    public IVaultClient Client => client;

    /// <summary>
    /// Recrea el <see cref="IVaultClient"/> con credenciales frescas.
    /// Para Kubernetes Auth lee un nuevo JWT del service account; para Token/AppRole usa las credenciales configuradas.
    /// </summary>
    /// <param name="options">Opciones de configuración de Vault.</param>
    public void ReAuthenticate(VaultOptions options)
    {
        client = VaultClientFactory.Create(options);
    }
}
