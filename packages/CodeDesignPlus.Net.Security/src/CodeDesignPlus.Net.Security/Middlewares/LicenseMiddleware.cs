using System.Net;

namespace CodeDesignPlus.Net.Security.Middlewares;

/// <summary>
/// Middleware to validate the license of the application.
/// On cache miss, delegates to <see cref="ITenantCacheLoader"/> to hydrate the cache before validating.
/// </summary>
/// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
public class LicenseMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Initializes a new instance of <see cref="LicenseMiddleware"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var userContext = context.RequestServices.GetRequiredService<IUserContext>();
        var tenantId = userContext.Tenant;

        var cacheLoader = context.RequestServices.GetService<ITenantCacheLoader>();

        if (cacheLoader is not null)
            await cacheLoader.EnsureCachedAsync(tenantId, context.RequestAborted);

        var tenant = context.RequestServices.GetRequiredService<ITenant>();

        await tenant.SetTenantAsync(tenantId);

        if (!tenant.LicenseIsValid())
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return;
        }

        await _next(context);
    }
}
