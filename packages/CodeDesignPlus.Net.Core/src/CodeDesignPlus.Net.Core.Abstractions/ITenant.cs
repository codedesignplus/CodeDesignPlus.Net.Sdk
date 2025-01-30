namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Interface that defines the tenant identifier
/// </summary>
public interface ITenant
{
    /// <summary>
    /// Get the tenant identifier
    /// </summary>
    Guid Tenant { get; }
}
