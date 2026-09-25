using System.Net;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Guards;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Security.Middlewares;

/// <summary>
/// Loads the tenant of the current request into <see cref="ITenant"/>, so that handlers can read
/// its location, currency and licensed modules. Validating the license is a separate concern, left
/// to <see cref="LicenseMiddleware"/>.
/// </summary>
/// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
public class TenantContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Loads the tenant and continues the pipeline.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // X-Tenant siempre es un GUID. Antes, una cabecera como "abc" se leia como Guid.Empty y la peticion
        // seguia sin tenant, en silencio: ahora es 400 (pendings/028).
        var header = context.Request.Headers["X-Tenant"].ToString();

        Guard.IsTrue(!string.IsNullOrWhiteSpace(header) && !Guid.TryParse(header, out _), Layer.None, Errors.InvalidTenantHeader);

        var userContext = context.RequestServices.GetRequiredService<IUserContext>();
        var tenantId = userContext.Tenant;

        // Sin tenant no hay nada que cargar. Es lo normal en un entrypoint gRPC al que no le mandan
        // X-Tenant: el contexto es opcional, no una barrera.
        if (tenantId == Guid.Empty)
        {
            await _next(context);

            return;
        }

        var tenant = context.RequestServices.GetRequiredService<ITenant>();

        try
        {
            await tenant.SetAsync(tenantId, context.RequestAborted);
        }
        // Solo llega aqui un tenant que NO se pudo consultar: el que no existe sale del directorio como
        // CodeDesignPlusException (Errors.TenantNotFound) y el ExceptionMiddleware lo responde como 400.
        catch (SecurityException exception)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<TenantContextMiddleware>>();

            logger.LogError(exception, "The tenant {TenantId} could not be loaded for the current request", tenantId);

            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

            return;
        }

        // Con el tenant ya cargado, publicarlo para la telemetria no cuesta ninguna consulta extra:
        // el nombre viene en el mismo snapshot.
        TenantTelemetry.Seed(tenantId, tenant.Name);

        await _next(context);
    }
}
