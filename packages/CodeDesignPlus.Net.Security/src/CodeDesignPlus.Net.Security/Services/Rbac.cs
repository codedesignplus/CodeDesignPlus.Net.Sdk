using System.Collections.Concurrent;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Security.gRpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Service to manage the role-based access control of the application.
/// </summary>
public class Rbac : IRbac
{
    private readonly ConcurrentBag<RbacResource> Resources = [];
    private readonly gRpc.Rbac.RbacClient client;
    private readonly ILogger<Rbac> logger;
    private readonly GrpcChannel channel;
    private readonly SecurityOptions securityOptions;
    private readonly CoreOptions coreOptions;

    /// <summary>
    /// Initialize the service to manage the role-based access control of the application.
    /// </summary>
    /// <param name="logger">The logger service.</param>
    /// <param name="securityOptions">The security options of the application.</param>
    /// <param name="coreOptions">The core options of the application.</param>
    /// <param name="httpClientFactory">The factory to create http clients.</param>
    public Rbac(ILogger<Rbac> logger, IOptions<SecurityOptions> securityOptions, IOptions<CoreOptions> coreOptions, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        this.securityOptions = securityOptions.Value;
        this.coreOptions = coreOptions.Value;

        channel = GrpcChannel.ForAddress(this.securityOptions.ServerRbac, new GrpcChannelOptions
        {
            HttpClient = httpClientFactory.CreateClient()
        });

        client = new gRpc.Rbac.RbacClient(channel);

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

        Resources.Clear();

        foreach (var item in response.Resources)
        {
            Resources.Add(new RbacResource()
            {
                Controller = item.Controller,
                Action = item.Action,
                Method = item.Method,
                Role = item.Role
            });
        }

        this.logger.LogInformation("RbacService loaded, number of resources: {Count}", Resources.Count);
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
        var httpMethod = ConvertToEnum(httpVerb);

        var isAuthorized = Resources.Any(x => x.Controller == controller && x.Action == action && x.Method == httpMethod && roles.Contains(x.Role));

        this.logger.LogDebug("Role {Role} is authorized to access {Controller}/{Action} with {HttpVerb}: {IsAuthorized}", string.Join(",", roles), controller, action, httpVerb, isAuthorized);

        return Task.FromResult(isAuthorized);
    }

    /// <summary>
    /// Convert the string to the <see cref="gRpc.HttpMethod"/> enum.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns></returns>
    private static gRpc.HttpMethod ConvertToEnum(string value)
    {
        if (Enum.TryParse(typeof(gRpc.HttpMethod), value.ToUpper(), out var method))

            return (gRpc.HttpMethod)method;

        return gRpc.HttpMethod.None;
    }

    /// <summary>
    /// Release the resources used by the service.
    /// </summary>
    public void Dispose()
    {
        channel.ShutdownAsync().Wait();
    }
}
