using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Declara la cabecera <c>X-Tenant</c> en las operaciones que exigen usuario.
/// </summary>
/// <remarks>
/// La copropiedad no viaja en el token: la manda el cliente en <c>X-Tenant</c> y la lee <c>TenantContextMiddleware</c>.
/// Ninguna operación la declaraba, así que un cliente generado desde el Swagger no sabía que existe (pendings/753).
/// No se marca obligatoria porque hay operaciones de usuario que no son de una copropiedad (las copropiedades del
/// usuario, por ejemplo); el micro responde 400 si llega con un valor que no es un GUID.
/// </remarks>
public sealed class TenantHeaderOperationFilter : IOperationFilter
{
    /// <summary>
    /// El nombre de la cabecera.
    /// </summary>
    public const string HeaderName = "X-Tenant";

    /// <summary>
    /// Aplica el filtro a una operación.
    /// </summary>
    /// <param name="operation">La operación generada.</param>
    /// <param name="context">El contexto con la descripción de la acción.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata ?? [];

        if (metadata.OfType<IAllowAnonymous>().Any())
            return;

        operation.Parameters ??= [];

        if (operation.Parameters.Any(x => x.In == ParameterLocation.Header && string.Equals(x.Name, HeaderName, StringComparison.OrdinalIgnoreCase)))
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = HeaderName,
            In = ParameterLocation.Header,
            Required = false,
            Description = "Id of the property (tenant) the request acts on. Required by every operation on the data of a property.",
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
        });
    }
}
