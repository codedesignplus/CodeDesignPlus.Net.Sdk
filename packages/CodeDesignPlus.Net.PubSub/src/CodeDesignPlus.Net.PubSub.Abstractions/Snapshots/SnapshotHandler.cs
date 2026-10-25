using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

/// <summary>
/// Base del consumidor de un evento de foto: aplica a la copia local la foto que pidió esta aplicación y descarta las
/// que pidió otra.
/// </summary>
/// <remarks>
/// Declarar un consumidor que hereda de esta clase es lo que convierte la copia en sincronizable: al arrancar, el SDK
/// pide la foto completa que nombra la clave del evento (aplicación dueña, agregado y nombre de la foto)
/// (<typeparamref name="TSnapshot"/>) mientras la fuente no figure como sincronizada.
/// <para>
/// <see cref="ApplyAsync"/> hace upsert: crea la copia si no existe y, si existe, la reemplaza solo cuando
/// <see cref="SnapshotDomainEvent.Supersedes"/> dice que la foto es más nueva que lo último aplicado. Tiene que ser
/// idempotente: una petición repetida (dos réplicas, un reinicio) manda la misma foto otra vez.
/// </para>
/// <para>
/// Cada foto que <see cref="ApplyAsync"/> aplica sin lanzar se cuenta para su petición: la fuente solo queda
/// sincronizada cuando la copia aplicó tantas como anunció el dueño al cerrar. Una foto que lanza (y acaba en la cola de
/// descartados) deja la petición incompleta, y la foto se vuelve a pedir (pendings/485).
/// </para>
/// <para>
/// La foto de una copropiedad que ya no existe (dada de baja y a la espera de su purga) no se aplica: se descarta y
/// cuenta como aplicada, para que la petición cuadre (pendings/467).
/// </para>
/// </remarks>
/// <typeparam name="TSnapshot">El gemelo local del evento de foto del dueño.</typeparam>
/// <param name="coreOptions">Las opciones de la aplicación, de donde sale su nombre.</param>
public abstract class SnapshotHandler<TSnapshot>(IOptions<CoreOptions> coreOptions) : IEventHandler<TSnapshot>, ISnapshotProgressReporter where TSnapshot : SnapshotDomainEvent
{
    private ISnapshotProgressTracker progressTracker;
    private ISnapshotTenantFilter tenantFilter;

    /// <inheritdoc/>
    ISnapshotProgressTracker ISnapshotProgressReporter.ProgressTracker
    {
        set => this.progressTracker = value;
    }

    /// <inheritdoc/>
    ISnapshotTenantFilter ISnapshotProgressReporter.TenantFilter
    {
        set => this.tenantFilter = value;
    }

    /// <inheritdoc/>
    public async Task HandleAsync(TSnapshot data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!data.IsAddressedTo(coreOptions.Value.AppName))
        {
            return;
        }

        // La foto de una copropiedad que ya no existe no tiene dónde aplicarse, pero el dueño la contó al cerrar.
        var tenantMissing = this.tenantFilter is not null && data.Tenant != Guid.Empty && await this.tenantFilter.IsMissingAsync(data.Tenant, token).ConfigureAwait(false);

        if (!tenantMissing)
        {
            await this.ApplyAsync(data, token).ConfigureAwait(false);
        }

        // Solo cuenta lo que se aplicó sin lanzar (o lo descartado): una foto que falla no llega aquí y la petición queda
        // corta.
        if (this.progressTracker is not null)
        {
            await this.progressTracker.RecordAppliedAsync(data, token).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Aplica la foto a la copia local.
    /// </summary>
    /// <param name="snapshot">La foto del agregado.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    protected abstract Task ApplyAsync(TSnapshot snapshot, CancellationToken cancellationToken);
}
