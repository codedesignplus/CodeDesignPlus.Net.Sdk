namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Defines the base structure for DTOs.
/// </summary>
public interface IDtoBase
{
    /// <summary>
    /// Gets or sets the primary identifier of the record.
    /// </summary>
    Guid Id { get; set; }
}


/// <summary>
/// Defines the base structure for DTOs.
/// </summary>
public interface IDto : IDtoBase
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    Guid Tenant { get; set; }
}