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
    /// Get or sets the is active
    /// </summary>
    bool IsActive { get; set; }
    /// <summary>
    /// Get or sets the creatae at
    /// </summary>
    long CreatedAt { get; set; }
    /// <summary>
    /// Get or sets the create by
    /// </summary>
    Guid CreatedBy { get; set; }
    /// <summary>
    /// Get or sets the update at
    /// </summary>
    long? UpdatedAt { get; set; }
    /// <summary>
    /// Get or sets the update by
    /// </summary>
    Guid? UpdatedBy { get; set; }
    /// <summary>
    /// Get or set the tenant identifier.
    /// </summary>
    Guid Tenant { get; set; }
}