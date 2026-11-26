using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// El documento OpenAPI del que el gateway MCP genera las herramientas de los agentes.
/// </summary>
/// <remarks>
/// <para>
/// Se sirve en <c>/swagger/mcp/swagger.json</c>, junto al documento normal y cerrado igual hacia afuera por el ingress
/// (Kappali pendings/754). Lleva todas las operaciones de los controllers salvo las marcadas con
/// <see cref="ExcludeFromMcpAttribute"/>, y declara <c>servers: [{"url": "/"}]</c>: sin él, el gateway arma rutas con
/// <c>//</c>.
/// </para>
/// <para>
/// Cada operación lleva un <c>operationId</c> <c>Controlador_Acción</c> (en los dos documentos): el gateway lo usa como
/// nombre de la herramienta y no carga un micro si a alguna operación le falta.
/// </para>
/// </remarks>
public static class McpDocument
{
    /// <summary>
    /// El nombre del documento: la ruta queda <c>/swagger/mcp/swagger.json</c>.
    /// </summary>
    public const string Name = "mcp";

    /// <summary>
    /// Decide qué operaciones entran en cada documento.
    /// </summary>
    /// <param name="documentName">El documento que se genera.</param>
    /// <param name="description">La operación.</param>
    /// <returns>Si la operación entra en el documento.</returns>
    public static bool Includes(string documentName, ApiDescription description)
    {
        ArgumentNullException.ThrowIfNull(description);

        if (documentName == Name)
            return !IsExcluded(description);

        // El criterio de Swashbuckle para los demás documentos: sin grupo, entra en todos.
        return description.GroupName is null || description.GroupName == documentName;
    }

    /// <summary>
    /// El <c>operationId</c> de una operación: <c>Controlador_Acción</c>.
    /// </summary>
    /// <param name="description">La operación.</param>
    /// <returns>El id, o <see langword="null"/> si no es de un controller.</returns>
    public static string OperationId(ApiDescription description)
    {
        ArgumentNullException.ThrowIfNull(description);

        return description.ActionDescriptor is ControllerActionDescriptor action ? $"{action.ControllerName}_{action.ActionName}" : null;
    }

    private static bool IsExcluded(ApiDescription description) =>
        description.ActionDescriptor.EndpointMetadata?.OfType<ExcludeFromMcpAttribute>().Any() == true;
}

/// <summary>
/// Declara la raíz como servidor del documento del MCP.
/// </summary>
public sealed class McpServersDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Aplica el filtro al documento del MCP.
    /// </summary>
    /// <param name="swaggerDoc">El documento generado.</param>
    /// <param name="context">El contexto con el nombre del documento.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);
        ArgumentNullException.ThrowIfNull(context);

        if (context.DocumentName == McpDocument.Name)
            swaggerDoc.Servers = [new OpenApiServer { Url = "/" }];
    }
}
