using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;
using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Dice a la copia si la copropiedad de una foto ya no existe, según el directorio de copropiedades (pendings/467).
/// </summary>
/// <remarks>
/// Solo cuenta como inexistente lo que el directorio confirma así; si no se pudo consultar, la foto se aplica y, si falla,
/// sigue el camino de siempre (reintento y descartados).
/// </remarks>
/// <param name="directory">El directorio de copropiedades.</param>
/// <param name="logger">El logger.</param>
public sealed class SnapshotTenantFilter(ITenantDirectory directory, ILogger<SnapshotTenantFilter> logger) : ISnapshotTenantFilter
{
    /// <inheritdoc/>
    public async Task<bool> IsMissingAsync(Guid tenant, CancellationToken cancellationToken)
    {
        var lookup = await directory.LookupAsync(tenant, cancellationToken).ConfigureAwait(false);

        if (lookup.Status != Models.TenantLookupStatus.NotFound)
        {
            return false;
        }

        logger.LogInformation("Snapshot of tenant {TenantId} discarded: the tenant does not exist (deleted and awaiting its purge). It counts as applied.", tenant);

        return true;
    }
}
