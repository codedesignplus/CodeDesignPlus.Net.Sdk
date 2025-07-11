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
    /// Gets or sets the timestamp when the aggregate root was created.
    /// </summary>
    Instant CreatedAt { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user who created the aggregate root.
    /// </summary>
    Guid CreatedBy { get; set; }
    /// <summary>
    /// Gets or sets the timestamp when the aggregate root was last updated.
    /// </summary>
    Instant? UpdatedAt { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user who last updated the aggregate root.
    /// </summary>
    Guid? UpdatedBy { get; set; }
    /// <summary>
    /// Gets or sets the timestamp when the aggregate root was deleted.
    /// </summary>
    Instant? DeletedAt { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user who deleted the aggregate root.
    /// </summary>
    Guid? DeletedBy { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the aggregate root is deleted.
    /// This property is used to mark the aggregate root as deleted without physically removing it from the database.
    /// </summary>
    bool IsDeleted { get; set; }
}