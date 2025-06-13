namespace CodeDesignPlus.Net.Exceptions.Models;

/// <summary>
/// Represents a single invalid parameter detail, commonly used with RFC 9457.
/// </summary>
/// <param name="Name">The name of the invalid parameter (e.g., field name or query parameter name).</param>
/// <param name="Reason">A human-readable explanation of why the parameter is invalid.</param>
/// <param name="Code">An optional application-specific error code for this specific validation error.</param>
public record InvalidParamDetail(string Name, string Reason, string? Code = null);