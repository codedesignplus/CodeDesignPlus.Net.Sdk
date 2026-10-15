using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// Qué foto es: la aplicación dueña, el agregado y el nombre del evento de foto, sacados de su <see cref="EventKeyAttribute"/>.
/// </summary>
/// <remarks>
/// Un mismo agregado puede tener varias fotos, una por cada vista que copian otros (la deuda de un documento y lo que una
/// nota crédito le quita al ingreso salen del mismo agregado de facturación): por eso la foto se identifica por su
/// evento y no solo por el agregado.
/// </remarks>
/// <param name="Source">La aplicación dueña, o nulo si la clave no la nombra (el evento del propio dueño).</param>
/// <param name="Aggregate">El agregado: la entidad de la clave.</param>
/// <param name="Snapshot">El nombre del evento de foto: el evento de la clave.</param>
public sealed record SnapshotKey(string? Source, string Aggregate, string Snapshot)
{
    /// <summary>
    /// Lee la clave de un evento de foto.
    /// </summary>
    /// <param name="snapshotType">El evento de foto o su gemelo.</param>
    /// <returns>La clave.</returns>
    /// <exception cref="ArgumentException">El tipo no hereda de <see cref="SnapshotDomainEvent"/> o no tiene <see cref="EventKeyAttribute"/>.</exception>
    public static SnapshotKey Of(Type snapshotType)
    {
        ArgumentNullException.ThrowIfNull(snapshotType);

        if (!typeof(SnapshotDomainEvent).IsAssignableFrom(snapshotType))
        {
            throw new ArgumentException($"The type {snapshotType.Name} does not inherit from {nameof(SnapshotDomainEvent)}.", nameof(snapshotType));
        }

        var attribute = snapshotType.GetCustomAttribute<EventKeyAttribute>() ?? throw new ArgumentException($"The snapshot event {snapshotType.Name} has no EventKey.", nameof(snapshotType));

        return new SnapshotKey(attribute.AppName, attribute.Entity, attribute.Event);
    }
}
