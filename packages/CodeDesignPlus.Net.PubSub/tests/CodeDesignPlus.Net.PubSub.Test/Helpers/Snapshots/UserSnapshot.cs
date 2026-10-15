using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Attributes;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;

/// <summary>
/// Gemelo de la foto de un usuario, como lo declararia la copia: la clave nombra al dueño.
/// </summary>
[EventKey("UserEntity", 1, "UserSnapshotDomainEvent", "xunit-owner", "codedesignplus")]
public class UserSnapshotDomainEvent(Guid aggregateId, string name, Guid tenant, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : SnapshotDomainEvent(aggregateId, tenant, eventId, occurredAt, metadata)
{
    public string Name { get; private set; } = name;
}

/// <summary>
/// Fuente de fotos en memoria, con los usuarios ordenados por identificador.
/// </summary>
public class UserSnapshotSource : ISnapshotSource<UserEntity>
{
    public List<UserEntity> Users { get; } = [];

    public Task<IReadOnlyList<SnapshotDomainEvent>> GetPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken)
    {
        IReadOnlyList<SnapshotDomainEvent> result = Users
            .Where(x => tenant is null || x.Tenant == tenant)
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => (SnapshotDomainEvent)new UserSnapshotDomainEvent(x.Id, x.Name!, x.Tenant))
            .ToList();

        return Task.FromResult(result);
    }
}

/// <summary>
/// Consumidor de la foto en la copia: anota lo que aplica.
/// </summary>
[QueueName<UserEntity>("UserSnapshotHandler")]
public class UserSnapshotHandler(IOptions<CoreOptions> coreOptions) : SnapshotHandler<UserSnapshotDomainEvent>(coreOptions)
{
    public List<UserSnapshotDomainEvent> Applied { get; } = [];

    protected override Task ApplyAsync(UserSnapshotDomainEvent snapshot, CancellationToken cancellationToken)
    {
        this.Applied.Add(snapshot);

        return Task.CompletedTask;
    }
}
