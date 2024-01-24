namespace CodeDesignPlus.Net.Core.Abstractions;


/// <summary>
/// Defines the base structure for entities and DTOs.
/// </summary>
public interface IBase
{
    /// <summary>
    /// Gets or sets the primary identifier of the record.
    /// </summary>
    Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    Guid Tenant { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the record is active.
    /// </summary>
    bool IsActive { get; set; }
}