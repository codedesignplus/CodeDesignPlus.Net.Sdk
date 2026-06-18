namespace CodeDesignPlus.Net.AI.Abstractions.DocumentIntelligence;

/// <summary>
/// Result of a document analysis operation containing extracted fields.
/// </summary>
public record DocumentAnalysisResponse
{
    /// <summary>Whether the analysis completed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>Error message if the analysis failed.</summary>
    public string? Error { get; init; }

    /// <summary>The model ID that was used for analysis.</summary>
    public string ModelId { get; init; } = null!;

    /// <summary>List of fields extracted from the document.</summary>
    public List<ExtractedField> Fields { get; init; } = [];

    /// <summary>Average confidence score across all extracted fields (0.0 to 1.0).</summary>
    public float Confidence { get; init; }
}

/// <summary>
/// A single field extracted from a document with its value and confidence.
/// </summary>
/// <param name="Name">The field name (e.g., "FirstName", "DocumentNumber", "ExpirationDate").</param>
/// <param name="Value">The extracted value as text, or null if not detected.</param>
/// <param name="Confidence">Confidence score for this field (0.0 to 1.0).</param>
public record ExtractedField(string Name, string? Value, float Confidence);
