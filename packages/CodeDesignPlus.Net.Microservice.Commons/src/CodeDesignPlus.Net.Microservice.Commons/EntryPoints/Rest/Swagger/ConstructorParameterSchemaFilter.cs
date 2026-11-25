using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Quita <c>readOnly</c> a las propiedades que Newtonsoft llena por el constructor.
/// </summary>
/// <remarks>
/// Los value objects del dominio tienen setters privados y un constructor <c>[JsonConstructor]</c>: Newtonsoft los
/// deserializa por ese constructor, pero el esquema los marcaba de solo lectura, y OpenAPI dice que una propiedad
/// de solo lectura no se envía. Un cliente generado desde el Swagger (o un gateway MCP) las quitaba del body
/// (pendings/753). La fuente de verdad es el contrato de Newtonsoft con la configuración de MVC del micro: lo que
/// es parámetro de su creador se puede escribir.
/// </remarks>
/// <param name="options">La configuración de Newtonsoft de MVC, la misma con la que el micro deserializa.</param>
public sealed class ConstructorParameterSchemaFilter(IOptions<MvcNewtonsoftJsonOptions> options) : ISchemaFilter
{
    private readonly IContractResolver contractResolver = options.Value.SerializerSettings.ContractResolver ?? new DefaultContractResolver();

    /// <summary>
    /// Aplica el filtro al esquema de un tipo.
    /// </summary>
    /// <param name="schema">El esquema generado.</param>
    /// <param name="context">El contexto con el tipo del esquema.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (schema.Properties is null || schema.Properties.Count == 0)
            return;

        if (this.contractResolver.ResolveContract(context.Type) is not JsonObjectContract contract || contract.CreatorParameters.Count == 0)
            return;

        foreach (var parameter in contract.CreatorParameters)
        {
            var property = schema.Properties.FirstOrDefault(x => string.Equals(x.Key, parameter.PropertyName, StringComparison.OrdinalIgnoreCase)).Value;

            if (property is not null)
                property.ReadOnly = false;
        }
    }
}
