using System.Net;

namespace CodeDesignPlus.Net.Security.Middlewares;

/// <summary>
/// Middleware to validate the license of the application.
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
        var tenant = context.RequestServices.GetRequiredService<ITenant>();
        var userContext = context.RequestServices.GetRequiredService<IUserContext>();

        await tenant.SetTenantAsync(userContext.Tenant);

        if (!tenant.LicenseIsValid())
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            
            return;
        }

        await _next(context);
    }
}
