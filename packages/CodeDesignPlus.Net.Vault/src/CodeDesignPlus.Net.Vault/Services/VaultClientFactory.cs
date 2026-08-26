using System.Net.Http;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;

namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// Factory for creating instances of <see cref="IVaultClient"/>.
/// </summary>
public static class VaultClientFactory
{
    /// <summary>
    /// UN SOLO HttpClient PARA TODOS LOS CLIENTES QUE SE CREEN, y no es un detalle menor.
    ///
    /// VaultSharp construye un HttpClient propio dentro de Polymath por cada VaultClient, y
    /// VaultClient NO implementa IDisposable: no hay forma de liberarlo. Como ahora nos
    /// reautenticamos periodicamente —y cada reautenticacion es un VaultClient nuevo— sin esto
    /// el proceso iria acumulando un HttpClient y su pool de conexiones cada pocos minutos,
    /// durante dias. Compartiendolo, lo que se recrea es solo el envoltorio.
    ///
    /// PooledConnectionLifetime existe porque un HttpClient de vida infinita no se entera de que
    /// cambie el DNS: si el Service de Vault pasa a otra IP, las conexiones abiertas seguirian
    /// apuntando a la vieja. Cinco minutos las recicla sin coste apreciable.
    /// </summary>
    private static readonly HttpClient SharedHttpClient = new(new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5)
    })
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    /// <summary>
    /// Create a new instance of <see cref="IVaultClient"/> with the specified options.
    /// </summary>
    /// <param name="options">The options used to configure the Vault client.</param>
    /// <exception cref="ArgumentNullException">options is null.</exception>
    /// <returns>A new instance of <see cref="IVaultClient"/>.</returns>
    public static IVaultClient Create(VaultOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.TypeAuth == TypeAuth.Token)
            return Build(options, new TokenAuthMethodInfo(options.Token));

        if (options.TypeAuth == TypeAuth.AppRole)
            return Build(options, new AppRoleAuthMethodInfo(options.RoleId, options.SecretId));

        if (options.TypeAuth == TypeAuth.Kubernetes)
        {
            // EL JWT SE LEE AQUI, EN CADA LLAMADA, Y ESE ES EL PUNTO DE TODO ESTO.
            //
            // KubernetesAuthMethodInfo recibe el JWT como string y su propiedad es de solo
            // lectura: el credencial queda congelado durante toda la vida del cliente. Kubernetes
            // rota el token proyectado (expirationSeconds ~3607s), asi que cualquier login que
            // VaultSharp intente pasada esa hora usa un JWT muerto y Vault responde
            // "invalid expiration time (exp) claim: token is expired".
            //
            // Por eso la unica forma de refrescar el credencial es RECREAR EL CLIENTE. No se
            // puede inyectar un IAuthMethodLoginProvider propio: AuthProviderFactory es internal.
            var jwt = File.ReadAllText(options.Kubernetes.PathTokenKubernetes);

            var roleName = $"{options.Solution}-{options.Kubernetes.RoleSuffix}";

            return Build(options, new KubernetesAuthMethodInfo(roleName, jwt));
        }

        throw new VaultException("The authentication type is not defined.");
    }

    /// <summary>
    /// Builds the <see cref="IVaultClient"/> reusing the shared <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="options">The options used to configure the Vault client.</param>
    /// <param name="authMethod">The authentication method to use.</param>
    /// <returns>A new instance of <see cref="IVaultClient"/>.</returns>
    private static IVaultClient Build(VaultOptions options, IAuthMethodInfo authMethod)
    {
        return new VaultClient(new VaultClientSettings(options.Address, authMethod)
        {
            // Se ignora el handler que propone VaultSharp a proposito: el nuestro ya lleva el
            // PooledConnectionLifetime. Si algun dia se usa PostProcessHttpClientHandlerAction,
            // habra que aplicarlo aqui, porque por esta via no se ejecuta.
            MyHttpClientProviderFunc = _ => SharedHttpClient
        });
    }
}
