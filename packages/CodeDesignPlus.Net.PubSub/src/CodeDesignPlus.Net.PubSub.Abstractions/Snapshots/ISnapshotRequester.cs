using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

/// <summary>
/// Pide a mano la foto de una fuente: sirve para reparar una copia de una copropiedad sin esperar al siguiente arranque.
/// </summary>
/// <remarks>
/// Se registra cuando la aplicación tiene al menos un <see cref="SnapshotHandler{TSnapshot}"/>. El endpoint de soporte
/// que lo llame lo declara cada micro, con su permiso en el RBAC.
/// </remarks>
public interface ISnapshotRequester
{
    /// <summary>
    /// Publica la petición de foto de la fuente que nombra la clave de <typeparamref name="TSnapshot"/>.
    /// </summary>
    /// <typeparam name="TSnapshot">El gemelo local del evento de foto.</typeparam>
    /// <param name="tenant">La copropiedad a fotografiar, o nulo para todas.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El identificador de la petición.</returns>
    Task<Guid> RequestAsync<TSnapshot>(Guid? tenant, CancellationToken cancellationToken) where TSnapshot : SnapshotDomainEvent;

    /// <summary>
    /// Publica la petición de foto de la fuente que nombra la clave del tipo indicado.
    /// </summary>
    /// <param name="snapshotType">El gemelo local del evento de foto.</param>
    /// <param name="tenant">La copropiedad a fotografiar, o nulo para todas.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El identificador de la petición.</returns>
    Task<Guid> RequestAsync(Type snapshotType, Guid? tenant, CancellationToken cancellationToken);
}
