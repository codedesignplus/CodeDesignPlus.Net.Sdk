using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Dice en la descripción de la operación que los montos de su respuesta van en centavos.
/// </summary>
/// <remarks>
/// <see cref="MinorUnitsSchemaFilter"/> marca cada monto en el esquema, pero el gateway MCP (agentgateway v1.7.0) solo le
/// entrega al agente el esquema de entrada de cada herramienta, no el de la respuesta: la marca de una respuesta como
/// <c>totalDue: 706167868</c> no le llegaba y lo leía como pesos (Kappali pendings/754). La descripción sí le llega, así
/// que se avisa ahí, solo en las operaciones cuya respuesta exitosa lleva algún monto.
/// </remarks>
public sealed class MoneyResponseOperationFilter : IOperationFilter
{
    /// <summary>
    /// La frase que se añade a la descripción.
    /// </summary>
    public const string Notice = "Money amounts in the response are integers in the minor unit of the currency (cents): 12500050 is 125,000.50 COP.";

    /// <summary>
    /// Aplica el filtro a una operación.
    /// </summary>
    /// <param name="operation">La operación generada.</param>
    /// <param name="context">El contexto, con el repositorio de esquemas.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        if (operation.Responses is null)
            return;

        var success = operation.Responses
            .Where(x => x.Key.StartsWith('2'))
            .SelectMany(x => x.Value.Content?.Values ?? [])
            .Select(x => x.Schema)
            .Where(x => x is not null);

        if (!success.Any(schema => HasMoney(schema, context.SchemaRepository, [])))
            return;

        if (operation.Description?.Contains(Notice, StringComparison.Ordinal) == true)
            return;

        operation.Description = string.IsNullOrWhiteSpace(operation.Description) ? Notice : $"{operation.Description}\n\n{Notice}";
    }

    private static bool HasMoney(OpenApiSchema schema, SchemaRepository repository, HashSet<string> visited)
    {
        if (schema is null)
            return false;

        if (schema.Extensions?.ContainsKey(MinorUnitsSchemaFilter.Extension) == true)
            return true;

        if (schema.Reference is not null)
        {
            if (!visited.Add(schema.Reference.Id) || !repository.Schemas.TryGetValue(schema.Reference.Id, out var referenced))
                return false;

            return HasMoney(referenced, repository, visited);
        }

        return (schema.Properties?.Values.Any(x => HasMoney(x, repository, visited)) ?? false)
            || HasMoney(schema.Items, repository, visited)
            || (schema.AllOf?.Any(x => HasMoney(x, repository, visited)) ?? false)
            || (schema.OneOf?.Any(x => HasMoney(x, repository, visited)) ?? false)
            || (schema.AnyOf?.Any(x => HasMoney(x, repository, visited)) ?? false)
            || HasMoney(schema.AdditionalProperties, repository, visited);
    }
}
