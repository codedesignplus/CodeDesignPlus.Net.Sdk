using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Publica peticiones de foto en nombre de esta aplicación.
/// </summary>
internal sealed class SnapshotRequester(IMessage message, IOptions<CoreOptions> coreOptions) : ISnapshotRequester
{
    /// <inheritdoc/>
    public Task<Guid> RequestAsync<TSnapshot>(Guid? tenant, CancellationToken cancellationToken) where TSnapshot : SnapshotDomainEvent
    {
        return this.RequestAsync(typeof(TSnapshot), tenant, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Guid> RequestAsync(Type snapshotType, Guid? tenant, CancellationToken cancellationToken)
    {
        var key = SnapshotSync.KeyOf(snapshotType);

        var request = SnapshotRequestedDomainEvent.Create(key, coreOptions.Value.AppName, tenant);

        await message.PublishAsync(request, cancellationToken).ConfigureAwait(false);

        return request.RequestId;
    }
}
