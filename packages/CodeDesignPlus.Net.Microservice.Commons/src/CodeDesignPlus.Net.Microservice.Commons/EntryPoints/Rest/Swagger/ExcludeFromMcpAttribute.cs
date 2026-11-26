namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Saca un controller o una acción del documento OpenAPI del MCP, sin tocar el Swagger normal.
/// </summary>
/// <remarks>
/// El documento <c>/swagger/mcp/swagger.json</c> es del que el gateway MCP genera las herramientas para los agentes
/// (Kappali pendings/754). Lleva todas las operaciones salvo las marcadas con este atributo, igual que
/// <c>[AllowAnonymous]</c> se pone en el controller o en la acción.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class ExcludeFromMcpAttribute : Attribute
{
}
