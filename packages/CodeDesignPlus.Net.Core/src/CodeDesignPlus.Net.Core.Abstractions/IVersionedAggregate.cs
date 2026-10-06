namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Marks an aggregate whose persistence uses optimistic concurrency: every write checks the version it read.
/// </summary>
/// <remarks>
/// Opt-in, aggregate by aggregate. Implement it on any document that two processes can write at the same time (two
/// consumers, two replicas, a consumer and a job): a replace that does not check the version silently erases what the
/// other process saved in between.
/// <para>
/// The implementing class declares <c>public long Version { get; private set; }</c>. The repository is the only one that
/// changes it, through the private setter; the domain never touches it. A stored document without the field reads as
/// version 0.
/// </para>
/// </remarks>
public interface IVersionedAggregate : IEntityBase
{
    /// <summary>
    /// Gets the number of writes the stored document has received; 0 for an aggregate that has never been saved.
    /// </summary>
    long Version { get; }
}
