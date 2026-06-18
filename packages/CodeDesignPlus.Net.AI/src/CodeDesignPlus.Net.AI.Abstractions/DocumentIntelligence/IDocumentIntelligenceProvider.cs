using CodeDesignPlus.Net.AI.Abstractions.Options;

namespace CodeDesignPlus.Net.AI.Abstractions.DocumentIntelligence;

/// <summary>
/// Strategy interface for document intelligence providers.
/// Each provider (Azure Document Intelligence, AWS Textract, GCP Document AI)
/// implements this interface and is resolved by name at runtime.
/// </summary>
public interface IDocumentIntelligenceProvider
{
    /// <summary>
    /// The unique name of this provider (e.g., "AzureDocumentIntelligence", "AWSTextract").
    /// Used for named resolution in the DI container.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Analyzes a document stream using the specified model and returns extracted fields.
    /// </summary>
    /// <param name="document">The document content as a stream (image or PDF).</param>
    /// <param name="modelId">The model to use for analysis (e.g., "prebuilt-idDocument", "prebuilt-document").</param>
    /// <param name="options">Provider-specific configuration (endpoint, API key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The analysis result with extracted fields and confidence scores.</returns>
    Task<DocumentAnalysisResponse> AnalyzeAsync(
        Stream document,
        string modelId,
        DocumentIntelligenceProviderOptions options,
        IReadOnlyList<string>? queryFields = null,
        CancellationToken cancellationToken = default
    );
}
