using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// La fuente que nombra un evento de foto: la aplicación dueña y el agregado, sacados de su <see cref="EventKeyAttribute"/>.
/// </summary>
/// <param name="Source">La aplicación dueña (la aplicación de la clave).</param>
/// <param name="Aggregate">El agregado (la entidad de la clave).</param>
internal sealed record SnapshotSourceKey(string Source, string Aggregate)
{
    /// <summary>
    /// Lee la fuente de un evento de foto.
    /// </summary>
    /// <param name="snapshotType">El gemelo local del evento de foto.</param>
    /// <returns>La fuente.</returns>
    /// <exception cref="PubSubException">El tipo no es un evento de foto, no tiene clave o su clave no nombra la aplicación dueña.</exception>
    public static SnapshotSourceKey Of(Type snapshotType)
    {
        ArgumentNullException.ThrowIfNull(snapshotType);

        if (!typeof(SnapshotDomainEvent).IsAssignableFrom(snapshotType))
        {
            throw new PubSubException($"The type {snapshotType.Name} does not inherit from {nameof(SnapshotDomainEvent)}.");
        }

        var attribute = snapshotType.GetCustomAttribute<EventKeyAttribute>();

        // Sin la aplicación en la clave, la copia no sabe a quién pedirle: un gemelo siempre nombra a su dueño.
        if (attribute is null || string.IsNullOrWhiteSpace(attribute.AppName))
        {
            throw new PubSubException($"The snapshot event {snapshotType.Name} must declare the owner application in its EventKey.");
        }

        return new SnapshotSourceKey(attribute.AppName, attribute.Entity);
    }
}
