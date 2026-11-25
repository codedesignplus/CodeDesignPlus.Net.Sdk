using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Dice en qué unidad va el importe de <c>Money</c> y de <c>MoneyInput</c>.
/// </summary>
/// <remarks>
/// Los dos tienen un <c>amount</c> y no significan lo mismo: el de entrada es la unidad mayor (125000.50), el de
/// salida la unidad menor (12500050). El esquema no lo decía y es el error más caro que puede cometer quien llama
/// (pendings/753). Se reconocen por el nombre completo para no hacer depender este paquete de los value objects.
/// </remarks>
public sealed class MoneySchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Nombre completo del value object de dinero, en unidad menor.
    /// </summary>
    public const string MoneyType = "CodeDesignPlus.Net.ValueObjects.Financial.Money";

    /// <summary>
    /// Nombre completo de la entrada de dinero, en unidad mayor.
    /// </summary>
    public const string MoneyInputType = "CodeDesignPlus.Net.ValueObjects.Financial.MoneyInput";

    /// <summary>
    /// Aplica el filtro al esquema de <c>Money</c> o <c>MoneyInput</c>.
    /// </summary>
    /// <param name="schema">El esquema generado.</param>
    /// <param name="context">El contexto con el tipo del esquema.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        var description = context.Type.FullName switch
        {
            MoneyType => "Amount in the minor unit of the currency (cents): 12500050 is 125,000.50 COP.",
            MoneyInputType => "Amount in the major unit of the currency, with decimals: 125000.50 is 125,000.50 COP.",
            _ => null
        };

        if (description is null || schema.Properties is null)
            return;

        var amount = schema.Properties.FirstOrDefault(x => string.Equals(x.Key, "amount", StringComparison.OrdinalIgnoreCase)).Value;

        if (amount is not null)
            amount.Description = description;

        var currency = schema.Properties.FirstOrDefault(x => string.Equals(x.Key, "currency", StringComparison.OrdinalIgnoreCase)).Value;

        if (currency is not null)
            currency.Description = "ISO 4217 currency code, such as COP.";
    }
}
