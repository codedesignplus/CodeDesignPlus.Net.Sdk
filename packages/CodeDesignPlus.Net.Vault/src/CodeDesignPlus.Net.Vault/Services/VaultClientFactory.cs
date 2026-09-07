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
    /// SE COMPARTE EL HANDLER, NO EL HttpClient, y la distincion es la que importa.
    ///
    /// El handler es quien tiene el pool de sockets; el HttpClient es un envoltorio barato. Como
    /// ahora nos reautenticamos periodicamente —y cada reautenticacion construye un VaultClient
    /// nuevo, que ademas NO implementa IDisposable— sin compartir nada el proceso iria acumulando
    /// un pool de conexiones cada pocos minutos durante dias.
    ///
    /// COMPARTIR EL HttpClient ENTERO NO VALE, y costo una caida: Polymath asigna BaseAddress en
    /// su constructor, y HttpClient prohibe modificar propiedades una vez ha enviado la primera
    /// peticion. El primer cliente lo configuraba y lo usaba —el proveedor de configuracion lee
    /// los secretos al arrancar— y el segundo, el de AddVault sobre IServiceCollection, moria con
    /// "This instance has already started one or more requests. Properties can only be modified
    /// before sending the first request." Un HttpClient por VaultClient sobre un handler comun da
    /// las dos cosas: BaseAddress propio y sockets compartidos.
    ///
    /// PooledConnectionLifetime existe porque un pool de vida infinita no se entera de que cambie
    /// el DNS: si el Service de Vault pasa a otra IP, las conexiones abiertas seguirian apuntando
    /// a la vieja. Cinco minutos las recicla sin coste apreciable.
    /// </summary>
    private static readonly SocketsHttpHandler SharedHandler = new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5)
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
    /// Builds the <see cref="IVaultClient"/> with its own <see cref="HttpClient"/> over the shared handler.
    /// </summary>
    /// <param name="options">The options used to configure the Vault client.</param>
    /// <param name="authMethod">The authentication method to use.</param>
    /// <returns>A new instance of <see cref="IVaultClient"/>.</returns>
    private static IVaultClient Build(VaultOptions options, IAuthMethodInfo authMethod)
    {
        return new VaultClient(new VaultClientSettings(options.Address, authMethod)
        {
            // disposeHandler: false es obligatorio. El HttpClient no es dueno del handler, lo
            // toma prestado: si lo liberase al recogerlo el GC, se llevaria por delante el pool
            // compartido y con el las conexiones de los demas clientes.
            //
            // Se ignora el handler que propone VaultSharp a proposito: el nuestro ya lleva el
            // PooledConnectionLifetime. Si algun dia se usa PostProcessHttpClientHandlerAction,
            // habra que aplicarlo aqui, porque por esta via no se ejecuta.
            MyHttpClientProviderFunc = _ => new HttpClient(SharedHandler, disposeHandler: false)
            {
                Timeout = TimeSpan.FromSeconds(30)
            }
        });
    }
}
