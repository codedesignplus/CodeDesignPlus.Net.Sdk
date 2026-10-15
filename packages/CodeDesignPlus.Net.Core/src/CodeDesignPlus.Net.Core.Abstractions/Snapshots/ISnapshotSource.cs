namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// El dueño de un agregado sabe fotografiarlo: devuelve, página a página, un evento de foto por cada agregado vivo.
/// </summary>
/// <remarks>
/// Implemente <see cref="ISnapshotSource{TSnapshot}"/>, no esta interfaz: el agregado y el nombre de la foto salen de la
/// clave del evento. El SDK la descubre y la registra sola (con <c>PubSub:RegisterAutomaticHandlers</c>), y contesta con
/// ella las peticiones que nombran a la aplicación como dueña y a esa foto.
/// </remarks>
public interface ISnapshotSource
{
    /// <summary>
    /// El nombre del agregado, igual a la entidad de la clave del evento de foto.
    /// </summary>
    string Aggregate { get; }

    /// <summary>
    /// El nombre del evento de foto, igual al evento de su clave.
    /// </summary>
    string Snapshot { get; }

    /// <summary>
    /// Lee una página de agregados vivos y devuelve su foto.
    /// </summary>
    /// <param name="tenant">La copropiedad a leer, o nulo para todas.</param>
    /// <param name="page">La página, empezando en 1.</param>
    /// <param name="pageSize">Cuántos agregados por página.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>Las fotos de la página; menos de <paramref name="pageSize"/> indica que es la última.</returns>
    Task<IReadOnlyList<SnapshotDomainEvent>> ReadPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken);
}

/// <summary>
/// El dueño sabe fotografiar sus agregados con el evento <typeparamref name="TSnapshot"/>.
/// </summary>
/// <typeparam name="TSnapshot">El evento de foto, declarado con <c>[EventKey&lt;TAggregate&gt;(1, "XSnapshotDomainEvent")]</c>.</typeparam>
public interface ISnapshotSource<TSnapshot> : ISnapshotSource where TSnapshot : SnapshotDomainEvent
{
    /// <summary>
    /// Lee una página de agregados vivos y devuelve su foto.
    /// </summary>
    /// <param name="tenant">La copropiedad a leer, o nulo para todas.</param>
    /// <param name="page">La página, empezando en 1.</param>
    /// <param name="pageSize">Cuántos agregados por página.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>Las fotos de la página; menos de <paramref name="pageSize"/> indica que es la última.</returns>
    /// <remarks>
    /// El orden tiene que ser estable entre páginas (por identificador, por ejemplo). Lo que se borre o se cree mientras
    /// se pagina lo cubren los eventos en vivo.
    /// </remarks>
    Task<IReadOnlyList<TSnapshot>> GetPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken);

    /// <inheritdoc/>
    string ISnapshotSource.Aggregate => SnapshotKey.Of(typeof(TSnapshot)).Aggregate;

    /// <inheritdoc/>
    string ISnapshotSource.Snapshot => SnapshotKey.Of(typeof(TSnapshot)).Snapshot;

    /// <inheritdoc/>
    async Task<IReadOnlyList<SnapshotDomainEvent>> ISnapshotSource.ReadPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken)
    {
        var snapshots = await this.GetPageAsync(tenant, page, pageSize, cancellationToken).ConfigureAwait(false);

        return [.. snapshots];
    }
}
