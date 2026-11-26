namespace CodeDesignPlus.Net.Core.Abstractions.Attributes;

/// <summary>
/// Saca de la convención de centavos un <c>long</c> que no es dinero (regla 74).
/// </summary>
/// <remarks>
/// En la API todo <c>long</c> es dinero en unidad menor, como manda la regla 04, y el Swagger lo declara así sin que
/// nadie lo marque (Kappali pendings/754). La excepción se marca con este atributo al declarar la propiedad:
/// consecutivos y rangos de numeración, cupos, duraciones en milisegundos, tamaños en bytes.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
public sealed class NotMoneyAttribute : Attribute
{
}
