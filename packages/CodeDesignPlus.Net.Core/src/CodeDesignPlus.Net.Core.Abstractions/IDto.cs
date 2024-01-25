namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Defines the base structure for DTOs.
/// </summary>
public interface IDtoBase : IBase { }


/// <summary>
/// Defines the base structure for DTOs.
/// </summary>
public interface IDto : IDtoBase, IBase
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    Guid Tenant { get; set; }
}