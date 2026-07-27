using System.Net;

namespace CodeDesignPlus.Net.Security.Middlewares;

/// <summary>
/// Validates that the license of the current tenant is in force. Loading the tenant is a separate
/// concern, handled upstream by <see cref="TenantContextMiddleware"/>.
/// </summary>
/// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
public class LicenseMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Validates the license and continues the pipeline.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var tenant = context.RequestServices.GetRequiredService<ITenant>();

        if (!tenant.IsLoaded || !tenant.LicenseIsValid())
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return;
        }

        await _next(context);
    }
}
