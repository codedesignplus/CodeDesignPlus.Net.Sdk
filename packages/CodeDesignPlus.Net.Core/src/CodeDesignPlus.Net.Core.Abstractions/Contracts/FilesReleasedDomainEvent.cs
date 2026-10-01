using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Abstractions.Contracts;

/// <summary>
/// A microservice no longer uses some files it uploaded to ms-filestorage: the record that held them was deleted, or
/// they were removed from it.
/// </summary>
/// <remarks>
/// ms-filestorage consumes it and deactivates the files, which its cleanup job deletes after the retention period.
/// <para>
/// It is a platform contract, not an event of the publisher: the key names ms-filestorage, so every microservice
/// publishes to the same topic and ms-filestorage subscribes once, without depending on who uses its files. Do not
/// declare a copy in a microservice: a copy with a different key publishes to a topic nobody reads.
/// </para>
/// </remarks>
/// <param name="aggregateId">The record that released the files. Only for tracing.</param>
/// <param name="files">The ids of the released files, as ms-filestorage returned them on upload.</param>
/// <param name="releasedBy">The user who deleted the record or removed the files.</param>
/// <param name="tenant">The tenant that owns the files, or <see cref="Guid.Empty"/> for platform files.</param>
/// <param name="eventId">The identifier of the event (optional).</param>
/// <param name="occurredAt">The date and time when the event occurred (optional).</param>
/// <param name="metadata">Additional metadata associated with the event (optional).</param>
[EventKey("FileStorageAggregate", 1, "FilesReleasedDomainEvent", "ms-filestorage", "codedesignplus")]
public class FilesReleasedDomainEvent(
    Guid aggregateId,
    List<Guid> files,
    Guid releasedBy,
    Guid tenant,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata), ITenant
{
    /// <summary>
    /// Gets the ids of the released files.
    /// </summary>
    public List<Guid> Files { get; private set; } = files;

    /// <summary>
    /// Gets the user who deleted the record or removed the files.
    /// </summary>
    public Guid ReleasedBy { get; private set; } = releasedBy;

    /// <summary>
    /// Gets the tenant that owns the files, or <see cref="Guid.Empty"/> for platform files.
    /// </summary>
    public Guid Tenant { get; private set; } = tenant;

    /// <summary>
    /// Creates the event.
    /// </summary>
    /// <param name="aggregateId">The record that released the files.</param>
    /// <param name="files">The ids of the released files.</param>
    /// <param name="releasedBy">The user who deleted the record or removed the files.</param>
    /// <param name="tenant">The tenant that owns the files, or <see cref="Guid.Empty"/> for platform files.</param>
    /// <returns>A new <see cref="FilesReleasedDomainEvent"/>.</returns>
    public static FilesReleasedDomainEvent Create(Guid aggregateId, IEnumerable<Guid> files, Guid releasedBy, Guid tenant)
    {
        return new FilesReleasedDomainEvent(aggregateId, [.. files.Distinct()], releasedBy, tenant);
    }
}
