using System.Net;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

namespace CodeDesignPlus.Net.Security.MIddlewares;

/// <summary>
/// Middleware to validate the permissions of the user based on the roles assigned to it.
/// </summary>
/// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
public class RbacMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Initializes a new instance of <see cref="RbacMiddleware"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // El RBAC decide sobre las acciones de los controllers, que es lo que se concede en ms-rbac. Lo demas pasa:
        // - lo marcado [AllowAnonymous] es publico por diseno (el listado publico de licencias, por ejemplo);
        // - lo que no es de un controller no tiene recurso que buscar: /health/*, Swagger, servicios gRPC.
        //   Antes se leia routeData.Values["controller"] sin comprobarlo: en /health/live era nulo, el middleware
        //   lanzaba NullReferenceException, la sonda de arranque recibia 500 y el pod nunca quedaba listo (plan 036).
        if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null
            || !TryGetResource(context, out var controllerName, out var actionName))
        {
            await _next(context);

            return;
        }

        var userContext = context.RequestServices.GetRequiredService<IUserContext>();
        var rbacService = context.RequestServices.GetRequiredService<IRbac>();
        var roleDirectory = context.RequestServices.GetRequiredService<IRoleDirectory>();

        var httpMethod = context.Request.Method;

        // Los roles salen del directorio y no de userContext.Roles. Ese claim lo llena el proveedor de
        // identidad con todos los grupos del usuario en todo el directorio, porque el proveedor no sabe
        // que es una copropiedad: autorizar con el le daria a quien administra una copropiedad los
        // mismos permisos en todas las demas a las que pertenece.
        var roles = await roleDirectory.GetRolesAsync(userContext.IdUser, userContext.Tenant, context.RequestAborted);

        var isAuthorized = await rbacService.IsAuthorizedAsync(controllerName, actionName, httpMethod, roles);

        if (!isAuthorized)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return;
        }

        await _next(context);
    }

    /// <summary>
    /// El controller y la accion de la peticion, si la atiende un controller.
    /// </summary>
    private static bool TryGetResource(HttpContext context, out string controller, out string action)
    {
        var values = context.GetRouteData().Values;

        controller = values.TryGetValue("controller", out var c) ? c?.ToString() : null;
        action = values.TryGetValue("action", out var a) ? a?.ToString() : null;

        return !string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action);
    }
}
