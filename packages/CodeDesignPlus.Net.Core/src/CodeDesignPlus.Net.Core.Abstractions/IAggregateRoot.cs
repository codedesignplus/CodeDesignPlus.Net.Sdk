namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a root entity in a domain-driven design context. Aggregate roots are the primary 
/// entry points to aggregates, a cluster of domain objects that are treated as a single unit for 
/// data changes.
/// </summary>
public interface IAggregateRoot : IBase
{
    /// <summary>
    /// Gets or sets the version of the aggregate.
    /// </summary>
    int Version { get; set; }
}

/// <summary>
/// Represents a typed aggregate root entity in a domain-driven design context with specified key and user key types.
/// </summary>
/// <typeparam name="TKey">The type of the primary key for this entity.</typeparam>
/// <typeparam name="TUserKey">The type of the user key for this entity.</typeparam>
public interface IAggregateRoot<TKey, TUserKey> : IBase<TKey, TUserKey>, IAggregateRoot
{

}