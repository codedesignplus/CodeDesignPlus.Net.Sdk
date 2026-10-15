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
/// </remarks>
/// <typeparam name="TSnapshot">El gemelo local del evento de foto del dueño.</typeparam>
/// <param name="coreOptions">Las opciones de la aplicación, de donde sale su nombre.</param>
public abstract class SnapshotHandler<TSnapshot>(IOptions<CoreOptions> coreOptions) : IEventHandler<TSnapshot> where TSnapshot : SnapshotDomainEvent
{
    /// <inheritdoc/>
    public Task HandleAsync(TSnapshot data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!data.IsAddressedTo(coreOptions.Value.AppName))
        {
            return Task.CompletedTask;
        }

        return this.ApplyAsync(data, token);
    }

    /// <summary>
    /// Aplica la foto a la copia local.
    /// </summary>
    /// <param name="snapshot">La foto del agregado.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    protected abstract Task ApplyAsync(TSnapshot snapshot, CancellationToken cancellationToken);
}
