namespace CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

/// <summary>
/// Dice si la copropiedad (o el inquilino) de una foto ya no existe, para que la copia la descarte en vez de mandarla a la
/// cola de descartados (pendings/467).
/// </summary>
/// <remarks>
/// Una copropiedad dada de baja sigue teniendo datos en el dueño hasta que se purga, así que el dueño publica sus fotos
/// cuando una copia pide la foto completa. La copia no puede aplicarlas: su contexto no carga una copropiedad que no
/// existe. Si cada una acabara en los descartados, la petición nunca cuadraría y la copia la volvería a pedir cada hora
/// hasta la purga. Descartada, la foto cuenta como aplicada.
/// <para>
/// Lo implementa quien sabe qué copropiedades existen (<c>CodeDesignPlus.Net.Security</c>, con el contexto de copropiedad
/// encendido). Sin implementación registrada, la copia aplica todas las fotos como antes.
/// </para>
/// </remarks>
public interface ISnapshotTenantFilter
{
    /// <summary>
    /// Indica si la copropiedad de la foto está confirmada como inexistente.
    /// </summary>
    /// <param name="tenant">La copropiedad de la foto.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns><c>true</c> solo si se confirmó que no existe; si no se pudo consultar, <c>false</c>.</returns>
    Task<bool> IsMissingAsync(Guid tenant, CancellationToken cancellationToken);
}
