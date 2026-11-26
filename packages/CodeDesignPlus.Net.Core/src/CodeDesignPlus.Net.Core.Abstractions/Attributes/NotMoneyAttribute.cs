namespace CodeDesignPlus.Net.Core.Abstractions.Attributes;

/// <summary>
/// Saca de la convención de centavos un <c>long</c> que no es dinero (regla 74).
/// </summary>
/// <remarks>
/// En la API todo <c>long</c> es dinero en unidad menor, como manda la regla 04, y el Swagger lo declara así sin que
/// nadie lo marque (Kappali pendings/754). La excepción se marca con este atributo al declarar la propiedad:
/// consecutivos y rangos de numeración, cupos, duraciones en milisegundos, tamaños en bytes.
/// <para>
/// En un record posicional se escribe <c>[property: NotMoney]</c>: sin el destino iría al parámetro del constructor,
/// donde ni el Swagger ni el generador de DTO lo ven. Por eso el atributo no admite parámetros y el compilador lo exige.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class NotMoneyAttribute : Attribute
{
}
