namespace CodeDesignPlus.Net.AI.Abstractions.Models;

/// <summary>
/// Represents the response from an AI chat request.
/// </summary>
public record ChatResponse
{
    /// <summary>Gets the generated text content from the AI.</summary>
    public required string Content { get; init; }

    /// <summary>Gets whether the request completed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>Gets the error message if the request failed.</summary>
    public string? Error { get; init; }

    /// <summary>Gets the model identifier used for the response.</summary>
    public string? Model { get; init; }

    /// <summary>Gets the number of tokens in the input prompt.</summary>
    public int PromptTokens { get; init; }

    /// <summary>Gets the number of tokens in the generated completion.</summary>
    public int CompletionTokens { get; init; }

    /// <summary>Gets the total number of tokens consumed (prompt + completion).</summary>
    public int TotalTokens { get; init; }
}
