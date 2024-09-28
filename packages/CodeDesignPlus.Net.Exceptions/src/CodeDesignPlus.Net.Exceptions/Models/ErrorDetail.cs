namespace CodeDesignPlus.Net.Exceptions.Models;

/// <summary>
/// Represents the error details of a request.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Field">The field that contains the error.</param>
/// <param name="Message">The error message.</param>
public record ErrorDetail(string Code, string Field, string Message);