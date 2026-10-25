using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

/// <summary>
/// Cuenta las fotos que la copia aplicó con éxito, para que la fuente solo quede sincronizada cuando aplicó todas las que
/// anunció el dueño (pendings/485).
/// </summary>
/// <remarks>
/// Lo implementa <c>CodeDesignPlus.Net.PubSub</c> y se lo entrega a cada <see cref="SnapshotHandler{TSnapshot}"/> al
/// resolverlo del contenedor: así los consumidores de los micros no cambian de constructor.
/// </remarks>
internal interface ISnapshotProgressTracker
{
    /// <summary>
    /// Anota que la copia aplicó una foto con éxito.
    /// </summary>
    /// <param name="snapshot">La foto aplicada.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task RecordAppliedAsync(SnapshotDomainEvent snapshot, CancellationToken cancellationToken);

    /// <summary>
    /// Anota el cierre de una petición completa: la fuente queda sincronizada si la copia ya aplicó todo lo anunciado, y
    /// si no, incompleta.
    /// </summary>
    /// <param name="completed">El cierre del dueño.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task RecordCompletedAsync(SnapshotCompletedDomainEvent completed, CancellationToken cancellationToken);
}

/// <summary>
/// Un consumidor de fotos al que el contenedor le entrega el contador.
/// </summary>
internal interface ISnapshotProgressReporter
{
    /// <summary>
    /// El contador de fotos aplicadas, o nulo si la aplicación no lleva la cuenta (sin almacén, o en una prueba).
    /// </summary>
    ISnapshotProgressTracker ProgressTracker { set; }

    /// <summary>
    /// Quien dice si la copropiedad de una foto ya no existe, o nulo si la aplicación no lo sabe (pendings/467).
    /// </summary>
    ISnapshotTenantFilter TenantFilter { set; }
}
