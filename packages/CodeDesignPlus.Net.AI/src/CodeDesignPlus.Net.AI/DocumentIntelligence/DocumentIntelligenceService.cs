using CodeDesignPlus.Net.AI.Abstractions.DocumentIntelligence;
using CodeDesignPlus.Net.AI.Abstractions.Options;

namespace CodeDesignPlus.Net.AI.DocumentIntelligence;

/// <summary>
/// Context in the Strategy pattern for Document Intelligence.
/// Resolves the configured provider by name and delegates the analysis.
/// </summary>
public class DocumentIntelligenceService(
    IOptions<AIOptions> options,
    IEnumerable<IDocumentIntelligenceProvider> providers,
    ILogger<DocumentIntelligenceService> logger
) : IDocumentIntelligenceService
{
    public async Task<DocumentAnalysisResponse> AnalyzeAsync(
        Stream document,
        string modelId,
        IReadOnlyList<string>? queryFields = null,
        CancellationToken cancellationToken = default)
    {
        var diOptions = options.Value.DocumentIntelligence
            ?? throw new AIException("DocumentIntelligence is not configured in AIOptions.");

        var providerName = diOptions.DefaultProvider;

        if (!diOptions.Providers.TryGetValue(providerName, out var providerOptions))
            throw new AIException($"Document Intelligence provider '{providerName}' is not configured in Providers.");

        var provider = providers.FirstOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new AIException($"No implementation found for Document Intelligence provider '{providerName}'.");

        logger.LogInformation("Analyzing document with provider '{Provider}', model '{Model}', queryFields: {Fields}", providerName, modelId, queryFields?.Count ?? 0);

        try
        {
            return await provider.AnalyzeAsync(document, modelId, providerOptions, queryFields, cancellationToken);
        }
        catch (Exception ex) when (ex is not AIException)
        {
            logger.LogError(ex, "Document analysis failed with provider '{Provider}', model '{Model}'", providerName, modelId);

            return new DocumentAnalysisResponse
            {
                Success = false,
                Error = ex.Message,
                ModelId = modelId,
            };
        }
    }
}
