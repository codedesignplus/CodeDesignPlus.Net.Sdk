namespace CodeDesignPlus.Net.AI.Abstractions.DocumentIntelligence;

/// <summary>
/// Service that resolves the configured Document Intelligence provider and delegates
/// document analysis. Acts as the Context in the Strategy pattern.
/// </summary>
public interface IDocumentIntelligenceService
{
    /// <summary>
    /// Analyzes a document using the configured default provider.
    /// </summary>
    /// <param name="document">The document content as a stream.</param>
    /// <param name="modelId">The model to use (e.g., "prebuilt-idDocument").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted fields with confidence scores.</returns>
    Task<DocumentAnalysisResponse> AnalyzeAsync(
        Stream document,
        string modelId,
        IReadOnlyList<string>? queryFields = null,
        CancellationToken cancellationToken = default
    );
}
