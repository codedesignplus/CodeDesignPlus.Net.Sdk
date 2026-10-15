namespace CodeDesignPlus.Net.Core.Abstractions.Snapshots;

/// <summary>
/// El dueño de un agregado sabe fotografiarlo: devuelve, página a página, un evento de foto por cada agregado vivo.
/// </summary>
/// <remarks>
/// Implemente <see cref="ISnapshotSource{TAggregate}"/>, no esta interfaz: el nombre del agregado sale del tipo. El SDK
/// la descubre y la registra sola (con <c>PubSub:RegisterAutomaticHandlers</c>), y contesta con ella las peticiones que
/// nombran a la aplicación como dueña.
/// </remarks>
public interface ISnapshotSource
{
    /// <summary>
    /// El nombre del agregado, igual a la entidad de la clave de su evento de foto.
    /// </summary>
    string Aggregate { get; }

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
    Task<IReadOnlyList<SnapshotDomainEvent>> GetPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken);
}

/// <summary>
/// El dueño de <typeparamref name="TAggregate"/> sabe fotografiarlo.
/// </summary>
/// <typeparam name="TAggregate">El agregado del dueño.</typeparam>
public interface ISnapshotSource<TAggregate> : ISnapshotSource where TAggregate : IEntityBase
{
    /// <summary>
    /// El nombre del agregado: el del tipo <typeparamref name="TAggregate"/>.
    /// </summary>
    string ISnapshotSource.Aggregate => typeof(TAggregate).Name;
}
