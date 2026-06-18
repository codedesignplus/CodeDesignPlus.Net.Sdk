using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.AI.Abstractions.Options;

/// <summary>
/// Configuration options for AI services (chat providers and document intelligence).
/// Providers dictionary is optional - if empty, only DocumentIntelligence services will be registered.
/// </summary>
public class AIOptions : IValidatableObject
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public static readonly string Section = "AI";

    /// <summary>
    /// Gets or sets the default provider name used when an agent does not specify one.
    /// </summary>
    public string DefaultProvider { get; set; } = "Claude";

    /// <summary>
    /// Gets or sets the configured AI providers (keyed by name).
    /// </summary>
    public Dictionary<string, ProviderOptions> Providers { get; set; } = [];

    /// <summary>
    /// Gets or sets the configured AI agents (keyed by name).
    /// </summary>
    public Dictionary<string, AgentOptions> Agents { get; set; } = [];

    /// <summary>
    /// Gets or sets Document Intelligence configuration (OCR / document analysis).
    /// Null means the feature is disabled — no providers will be registered.
    /// </summary>
    public DocumentIntelligenceOptions? DocumentIntelligence { get; set; }

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Only validate chat-related config if providers are configured
        if (Providers.Count > 0)
        {
            // Validate DefaultProvider
            if (!string.IsNullOrEmpty(DefaultProvider) && !Providers.ContainsKey(DefaultProvider))
            {
                results.Add(new ValidationResult(
                    $"Default provider '{DefaultProvider}' is not configured in Providers.",
                    [nameof(DefaultProvider)]
                ));
            }

            // Validate Agent providers
            foreach (var (name, agent) in Agents)
            {
                var provider = agent.Provider ?? DefaultProvider;
                if (string.IsNullOrEmpty(provider))
                {
                    results.Add(new ValidationResult(
                        $"Agent '{name}' does not have a provider and no DefaultProvider is set.",
                        [nameof(Agents)]
                    ));
                }
                else if (!Providers.ContainsKey(provider))
                {
                    results.Add(new ValidationResult(
                        $"Agent '{name}' references provider '{provider}' which is not configured.",
                        [nameof(Agents)]
                    ));
                }
            }
        }

        return results;
    }
}

/// <summary>
/// Configuration for a specific AI provider (API key, model, endpoint).
/// </summary>
public class ProviderOptions
{
    /// <summary>Gets or sets the API key for authenticating with the provider.</summary>
    [Required]
    public string ApiKey { get; set; } = null!;

    /// <summary>Gets or sets the default model to use for this provider.</summary>
    public string Model { get; set; } = null!;

    /// <summary>Gets or sets the endpoint URL (required for Azure OpenAI).</summary>
    public string? Endpoint { get; set; }

    /// <summary>Gets or sets the API version (optional, for Azure OpenAI).</summary>
    public string? ApiVersion { get; set; }
}

/// <summary>
/// Configuration for a named AI agent with system prompt and generation parameters.
/// </summary>
public class AgentOptions
{
    /// <summary>Gets or sets the provider to use (overrides DefaultProvider if set).</summary>
    public string? Provider { get; set; }

    /// <summary>Gets or sets the model to use (overrides the provider's default model).</summary>
    public string? Model { get; set; }

    /// <summary>Gets or sets the system prompt that defines the agent's behavior.</summary>
    [Required]
    public string SystemPrompt { get; set; } = null!;

    /// <summary>Gets or sets the maximum number of tokens to generate.</summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>Gets or sets the sampling temperature (0.0 = deterministic, 1.0 = creative).</summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>Gets or sets the top-p nucleus sampling parameter.</summary>
    public double TopP { get; set; } = 1.0;
}

/// <summary>
/// Configuration for Document Intelligence (OCR / document analysis).
/// </summary>
public class DocumentIntelligenceOptions
{
    /// <summary>
    /// Gets or sets the default provider for document analysis.
    /// </summary>
    public string DefaultProvider { get; set; } = "AzureDocumentIntelligence";

    /// <summary>
    /// Gets or sets provider-specific configurations keyed by provider name.
    /// </summary>
    [Required]
    public Dictionary<string, DocumentIntelligenceProviderOptions> Providers { get; set; } = [];
}

/// <summary>
/// Configuration for a specific Document Intelligence provider.
/// </summary>
public class DocumentIntelligenceProviderOptions
{
    /// <summary>
    /// Gets or sets the service endpoint URL.
    /// </summary>
    [Required]
    public string Endpoint { get; set; } = null!;

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = null!;
}
