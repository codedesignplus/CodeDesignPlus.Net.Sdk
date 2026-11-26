using System.Globalization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Quita del documento del MCP los límites exclusivos en su forma booleana.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="FluentValidationSchemaFilter"/> describe un <c>GreaterThan(0)</c> como <c>minimum: 0</c> y
/// <c>exclusiveMinimum: true</c>, que es como lo escribe OpenAPI 3.0. El gateway MCP copia el esquema tal cual en el
/// <c>inputSchema</c> de la herramienta, y los clientes lo validan como JSON Schema 2020-12, donde
/// <c>exclusiveMinimum</c> es un número: Claude Code descartaba las 36 herramientas que tenían uno (Kappali
/// pendings/754).
/// </para>
/// <para>
/// Solo en el documento <see cref="McpDocument.Name"/>; el normal sigue siendo OpenAPI 3.0 correcto. En un entero el
/// límite se vuelve inclusivo y exacto (<c>&gt; 0</c> es <c>minimum: 1</c>). En un decimal no hay número equivalente:
/// se deja el límite inclusivo y la condición estricta va en la descripción.
/// </para>
/// </remarks>
public sealed class McpBoundsDocumentFilter : IDocumentFilter
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

        if (context.DocumentName != McpDocument.Name)
            return;

        var visited = new HashSet<OpenApiSchema>(ReferenceEqualityComparer.Instance);

        foreach (var schema in swaggerDoc.Components?.Schemas?.Values ?? [])
            Visit(schema, visited);

        foreach (var operation in swaggerDoc.Paths?.Values.SelectMany(x => x.Operations.Values) ?? [])
        {
            foreach (var parameter in operation.Parameters ?? [])
                Visit(parameter.Schema, visited);

            foreach (var content in operation.RequestBody?.Content?.Values ?? [])
                Visit(content.Schema, visited);

            foreach (var content in operation.Responses?.Values.SelectMany(x => x.Content?.Values ?? []) ?? [])
                Visit(content.Schema, visited);
        }
    }

    private static void Visit(OpenApiSchema schema, HashSet<OpenApiSchema> visited)
    {
        if (schema is null || !visited.Add(schema))
            return;

        Rewrite(schema);

        foreach (var property in schema.Properties?.Values ?? [])
            Visit(property, visited);

        Visit(schema.Items, visited);
        Visit(schema.AdditionalProperties, visited);

        foreach (var child in (schema.AllOf ?? []).Concat(schema.OneOf ?? []).Concat(schema.AnyOf ?? []))
            Visit(child, visited);
    }

    private static void Rewrite(OpenApiSchema schema)
    {
        var integer = schema.Type == "integer";

        if (schema.ExclusiveMinimum == true && schema.Minimum is { } minimum)
        {
            if (integer)
                schema.Minimum = minimum + 1;
            else
                Describe(schema, $"Must be greater than {Format(minimum)}.");
        }

        if (schema.ExclusiveMaximum == true && schema.Maximum is { } maximum)
        {
            if (integer)
                schema.Maximum = maximum - 1;
            else
                Describe(schema, $"Must be less than {Format(maximum)}.");
        }

        schema.ExclusiveMinimum = null;
        schema.ExclusiveMaximum = null;
    }

    private static void Describe(OpenApiSchema schema, string condition)
    {
        if (schema.Description?.Contains(condition, StringComparison.Ordinal) == true)
            return;

        schema.Description = string.IsNullOrWhiteSpace(schema.Description) ? condition : $"{schema.Description} {condition}";
    }

    private static string Format(decimal value) => value.ToString(CultureInfo.InvariantCulture);
}
