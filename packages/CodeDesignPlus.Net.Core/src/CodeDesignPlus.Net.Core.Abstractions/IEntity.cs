namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Defines the base structure for entities.
/// </summary>
public interface IEntityBase : IBase { }


/// <summary>
/// Defines the base structure for entities.
/// </summary>
public interface IEntity : IEntityBase, IBase
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    Guid Tenant { get; set; }
}