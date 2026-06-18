using System.Text.RegularExpressions;
using Azure;
using Azure.AI.DocumentIntelligence;
using CodeDesignPlus.Net.AI.Abstractions.DocumentIntelligence;
using CodeDesignPlus.Net.AI.Abstractions.Options;

namespace CodeDesignPlus.Net.AI.DocumentIntelligence;

/// <summary>
/// Azure Document Intelligence (formerly Form Recognizer) provider.
/// Supports prebuilt models (prebuilt-idDocument, prebuilt-document) and custom models.
/// When queryFields are provided, uses the AnalyzeDocumentOptions overload to guide extraction.
/// </summary>
public class AzureDocumentIntelligenceProvider(ILogger<AzureDocumentIntelligenceProvider> logger)
    : IDocumentIntelligenceProvider
{
    public string Name => "AzureDocumentIntelligence";

    public async Task<DocumentAnalysisResponse> AnalyzeAsync(
        Stream document,
        string modelId,
        DocumentIntelligenceProviderOptions options,
        IReadOnlyList<string>? queryFields = null,
        CancellationToken cancellationToken = default)
    {
        var credential = new AzureKeyCredential(options.ApiKey);
        var client = new DocumentIntelligenceClient(new Uri(options.Endpoint), credential);

        logger.LogDebug("Starting Azure DI analysis with model '{ModelId}', queryFields: {Fields}", modelId, queryFields?.Count ?? 0);

        using var memoryStream = new MemoryStream();
        await document.CopyToAsync(memoryStream, cancellationToken);
        var binaryData = BinaryData.FromBytes(memoryStream.ToArray());

        AnalyzeResult result;

        Dictionary<string, string>? sanitizedToOriginal = null;

        if (queryFields is not null && queryFields.Count > 0)
        {
            sanitizedToOriginal = new Dictionary<string, string>();
            var analyzeOptions = new AnalyzeDocumentOptions(modelId, binaryData);

            analyzeOptions.Features.Add(DocumentAnalysisFeature.QueryFields);
            foreach (var field in queryFields)
            {
                var sanitized = SanitizeFieldName(field);
                sanitizedToOriginal[sanitized] = field;
                analyzeOptions.QueryFields.Add(sanitized);
            }

            var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, analyzeOptions, cancellationToken);
            result = operation.Value;
        }
        else
        {
            var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, binaryData, cancellationToken);
            result = operation.Value;
        }

        var fields = new List<ExtractedField>();

        if (result.Documents is not null)
        {
            foreach (var doc in result.Documents)
            {
                if (doc.Fields is null) continue;

                foreach (var kvp in doc.Fields)
                {
                    var fieldName = sanitizedToOriginal is not null && sanitizedToOriginal.TryGetValue(kvp.Key, out var original)
                        ? original
                        : kvp.Key;

                    fields.Add(new ExtractedField(
                        fieldName,
                        kvp.Value.Content,
                        (float)(kvp.Value.Confidence ?? 0)
                    ));
                }
            }
        }

        logger.LogInformation(
            "Azure DI analysis completed: {FieldCount} fields extracted, avg confidence {Confidence:P0}",
            fields.Count,
            fields.Count > 0 ? fields.Average(f => f.Confidence) : 0
        );

        return new DocumentAnalysisResponse
        {
            Success = true,
            ModelId = modelId,
            Fields = fields,
            Confidence = fields.Count > 0 ? fields.Average(f => f.Confidence) : 0
        };
    }

    private static string SanitizeFieldName(string name)
    {
        return Regex.Replace(name, @"[^\p{L}\p{M}\p{N}]", "_");
    }
}
