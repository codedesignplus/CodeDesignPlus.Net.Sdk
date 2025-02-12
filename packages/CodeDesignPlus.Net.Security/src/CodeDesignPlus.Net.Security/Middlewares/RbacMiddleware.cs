using System.Net;
using CodeDesignPlus.Net.Core.Abstractions.Options;
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
        var userContext = context.RequestServices.GetRequiredService<IUserContext>();
        var coreOptions = context.RequestServices.GetRequiredService<IOptions<CoreOptions>>().Value;
        var rbacService = context.RequestServices.GetRequiredService<IRbac>();

        var routeData = context.GetRouteData();
        var controllerName = routeData.Values["controller"]?.ToString();
        var actionName = routeData.Values["action"]?.ToString();
        var httpMethod = context.Request.Method;

        var isAuthorized = await rbacService.IsAuthorizedAsync(controllerName, actionName, httpMethod, userContext.Roles);

        if (!isAuthorized)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return;
        }

        await _next(context);
    }
}
