namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the base interface for all DTOs (Data Transfer Objects).
/// </summary>
public interface IDtoBase
{
    /// <summary>
    /// Gets or sets the unique identifier of the DTO.
    /// </summary>
    Guid Id { get; set; }
}

/// <summary>
/// Represents an interface for a Data Transfer Object (DTO) with additional properties.
/// </summary>
public interface IDto : IDtoBase
{
    /// <summary>
    /// Gets or sets the unique identifier of the tenant.
    /// </summary>
    Guid Tenant { get; set; }
}