namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Defines the base structure for entities.
/// </summary>
public interface IEntityBase
{

    /// <summary>
    /// Gets or sets the primary identifier of the record.
    /// </summary>
    Guid Id { get; }
}


/// <summary>
/// Defines the base structure for entities.
/// </summary>
public interface IEntity : IEntityBase
{

    /// <summary>
    /// Get or sets the is active
    /// </summary>
    bool IsActive { get; set; }
    /// <summary>
    /// Get or sets the creatae at
    /// </summary>
    Instant CreatedAt { get; set; }
    /// <summary>
    /// Get or sets the create by
    /// </summary>
    Guid CreatedBy { get; set; }
    /// <summary>
    /// Get or sets the update at
    /// </summary>
    Instant? UpdatedAt { get; set; }
    /// <summary>
    /// Get or sets the update by
    /// </summary>
    Guid? UpdatedBy { get; set; }
}