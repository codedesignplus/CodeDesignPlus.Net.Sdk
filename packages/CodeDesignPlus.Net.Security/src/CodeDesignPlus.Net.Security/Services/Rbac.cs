using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Security.gRpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Service to manage the role-based access control of the application.
/// </summary>
/// <remarks>
/// Se registra como singleton: el servicio en segundo plano carga los permisos y el middleware los consulta en cada
/// peticion, y los dos tienen que ver la misma instancia. Registrado como scoped, el middleware recibia una instancia
/// nueva y vacia en cada peticion y negaba todo (plan 031 de pendings).
/// </remarks>
public class Rbac : IRbac
{
    /// <summary>
    /// Los permisos cargados. Se reemplaza la lista entera al refrescar, nunca se vacia y se rellena: asi una peticion
    /// concurrente ve la lista anterior o la nueva, pero nunca una vacia.
    /// </summary>
    private volatile IReadOnlyList<RbacResource> resources = [];
    private readonly gRpc.Rbac.RbacClient client;
    private readonly ILogger<Rbac> logger;
    private readonly CoreOptions coreOptions;

    /// <summary>
    /// Initialize the service to manage the role-based access control of the application.
    /// </summary>
    /// <param name="logger">The logger service.</param>
    /// <param name="coreOptions">The core options of the application.</param>
    /// <param name="client">The gRpc client to communicate with the Rbac service.</param>
    public Rbac(ILogger<Rbac> logger,  IOptions<CoreOptions> coreOptions, gRpc.Rbac.RbacClient client)
    {
        this.logger = logger;
        this.coreOptions = coreOptions.Value;
        this.client = client;

        this.logger.LogInformation("RbacService initialized");
    }

    /// <summary>
    /// Load the roles and permissions of the user.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Return a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task LoadRbacAsync(CancellationToken cancellationToken)
    {
        var response = await client.GetRbacAsync(new GetRbacRequest()
        {
            Microservice = coreOptions.AppName
        }, cancellationToken: cancellationToken);

        var loaded = response.Resources
            .Select(item => new RbacResource()
            {
                Controller = item.Controller,
                Action = item.Action,
                Method = item.Method,
                Role = item.Role
            })
            .ToList();

        resources = loaded;

        this.logger.LogInformation("RbacService loaded, number of resources: {Count}", loaded.Count);
    }
    
    /// <summary>
    /// Validate if the user has permission to access the controller and action.
    /// </summary>
    /// <param name="controller">The name of the controller to validate.</param>
    /// <param name="action">The name of the action to validate.</param>
    /// <param name="httpVerb">The HTTP verb to validate.</param>
    /// <param name="roles">The roles assigned to the user.</param>
    /// <returns>Return true if the user has permission to access the controller and action; otherwise, false.</returns>
    public Task<bool> IsAuthorizedAsync(string controller, string action, string httpVerb, string[] roles)
    {
        var httpMethod = ToHttpMethod(httpVerb);

        var isAuthorized = resources.Any(x => x.Controller == controller && x.Action == action && x.Method == httpMethod && roles.Contains(x.Role));

        this.logger.LogDebug("Role '{Role}' is {Authorized} to access the resource '{Controller}/{Action}' with the method '{HttpVerb}'", string.Join(",", roles), isAuthorized ? "authorized" : "not authorized", controller, action, httpVerb);

        return Task.FromResult(isAuthorized);
    }

    /// <summary>
    /// Traduce el metodo HTTP de la peticion (<c>HttpRequest.Method</c>, en mayusculas: "GET", "POST"...) al enum del
    /// gRPC. Con <c>Enum.TryParse</c> sin ignorar mayusculas "GET" no casaba con <c>Get</c>, el verbo quedaba en
    /// <c>None</c> y no se autorizaba nada (plan 031 de pendings).
    /// </summary>
    /// <param name="httpVerb">El metodo HTTP de la peticion.</param>
    /// <returns>El valor del enum, o <see cref="gRpc.HttpMethod.None"/> si no es un metodo conocido.</returns>
    internal static gRpc.HttpMethod ToHttpMethod(string httpVerb) => httpVerb?.ToUpperInvariant() switch
    {
        "GET" => gRpc.HttpMethod.Get,
        "POST" => gRpc.HttpMethod.Post,
        "PUT" => gRpc.HttpMethod.Put,
        "PATCH" => gRpc.HttpMethod.Patch,
        "DELETE" => gRpc.HttpMethod.Delete,
        _ => gRpc.HttpMethod.None
    };
}
