using System.Diagnostics;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Security.Middlewares;

/// <summary>
/// Applies the license of the current tenant to the REST request: in force or in its grace period everything goes
/// through; once the grace period ends the tenant is read-only. Loading the tenant is a separate concern, handled
/// upstream by <see cref="TenantContextMiddleware"/>.
/// </summary>
/// <remarks>
/// Before Kappali pendings/737 an expired tenant got a bare 403 on every route, including the ones it needed to pay
/// its renewal, and a request without tenant got one too. Now:
/// <list type="bullet">
/// <item>Grace: everything passes, marked with <see cref="LicenseHeaders.Status"/> and <see cref="LicenseHeaders.GraceEnds"/>.</item>
/// <item>Read-only: <c>GET</c>, <c>HEAD</c> and <c>OPTIONS</c> pass marked with <see cref="LicenseHeaders.Status"/>; any other
/// method gets 403 with a ProblemDetails (RFC 9457) carrying <see cref="Errors.LicenseExpired"/>.</item>
/// <item>Never blocked: gRPC calls between services, requests without tenant, the tenants of
/// <see cref="SecurityOptions.LicenseExemptTenants"/> and the paths of <see cref="SecurityOptions.LicenseExemptPaths"/>.</item>
/// </list>
/// Workers are not HTTP, so event consumers and jobs never go through here.
/// </remarks>
/// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
/// <param name="options">The security options.</param>
public class LicenseMiddleware(RequestDelegate next, IOptions<SecurityOptions> options)
{
    private static readonly Newtonsoft.Json.JsonSerializerSettings serializerSettings = JsonSerializer.CreateSettings(settings =>
    {
        settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        settings.Formatting = Newtonsoft.Json.Formatting.None;
    });

    private readonly RequestDelegate _next = next;
    private readonly SecurityOptions _options = options.Value;

    /// <summary>
    /// Applies the license and continues the pipeline when the request is allowed.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (this.IsExempt(context))
        {
            await _next(context);

            return;
        }

        var tenant = context.RequestServices.GetRequiredService<ITenant>();

        var standing = tenant.GetLicenseStanding(_options.GraceDays);

        if (standing.Status == LicenseStatus.Grace)
        {
            context.Response.Headers[LicenseHeaders.Status] = LicenseHeaders.GraceValue;
            context.Response.Headers[LicenseHeaders.GraceEnds] = InstantPattern.General.Format(standing.GraceEndsAt);
        }

        if (standing.Status == LicenseStatus.ReadOnly)
        {
            context.Response.Headers[LicenseHeaders.Status] = LicenseHeaders.ReadOnlyValue;

            if (!IsRead(context.Request.Method))
            {
                await WriteLicenseExpiredAsync(context);

                return;
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Whether the license does not apply to the request at all.
    /// </summary>
    private bool IsExempt(HttpContext context)
    {
        // gRPC entre servicios: lo llama otro micro (a veces en nombre de un worker), no la pantalla de la copropiedad.
        // Bloquearlo cortaria un cobro ya emitido o un pago que llega por webhook.
        if (context.Request.ContentType?.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (_options.LicenseExemptPaths.Any(path => context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var tenant = context.RequestServices.GetRequiredService<ITenant>();

        // Sin tenant no hay licencia que aplicar: un endpoint anonimo, la sonda de salud o una llamada de plataforma.
        if (!tenant.IsLoaded)
        {
            return true;
        }

        var userContext = context.RequestServices.GetRequiredService<IUserContext>();

        return _options.LicenseExemptTenants.Contains(userContext.Tenant);
    }

    private static bool IsRead(string method)
    {
        return HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);
    }

    /// <summary>
    /// Writes the 403 with the same ProblemDetails shape the ExceptionMiddleware of the microservices writes, so the
    /// client reads <c>error_code</c> in the same place.
    /// </summary>
    /// <remarks>
    /// It cannot throw and let the ExceptionMiddleware answer: that one turns every catalog error into a 400, and a
    /// read-only tenant is a refusal (403), not a malformed request.
    /// </remarks>
    private static Task WriteLicenseExpiredAsync(HttpContext context)
    {
        var language = ErrorLanguage.Current;

        var core = context.RequestServices.GetService<IOptions<CoreOptions>>()?.Value;

        var code = Errors.LicenseExpired.Code;

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = core is null ? "about:blank" : $"{core.ApiDocumentationBaseUrl}{core.AppName}/none-{code}",
            Title = Errors.LicenseExpiredTitle.GetMessage(language),
            Status = StatusCodes.Status403Forbidden,
            Detail = Errors.LicenseExpired.GetMessage(language),
            Instance = Activity.Current?.Id ?? context.TraceIdentifier
        };

        problemDetails.Extensions["layer"] = Layer.None.ToString();
        problemDetails.Extensions["error_code"] = code;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, serializerSettings));
    }
}
