using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.AI.Abstractions.Options;

/// <summary>
/// Configuration options for the AI service including providers and agents.
/// </summary>
public class AIOptions : IValidatableObject
{
    /// <summary>The configuration section name in appsettings.json.</summary>
    public static readonly string Section = "AI";

    /// <summary>Gets or sets the default provider name used when an agent does not specify one.</summary>
    public string DefaultProvider { get; set; } = "Claude";

    /// <summary>Gets or sets the configured AI providers (keyed by name).</summary>
    [Required]
    public Dictionary<string, ProviderOptions> Providers { get; set; } = [];

    /// <summary>Gets or sets the configured AI agents (keyed by name).</summary>
    public Dictionary<string, AgentOptions> Agents { get; set; } = [];

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Providers.Count == 0)
            results.Add(new ValidationResult("At least one AI provider must be configured.", [nameof(Providers)]));

        if (!string.IsNullOrEmpty(DefaultProvider) && !Providers.ContainsKey(DefaultProvider))
            results.Add(new ValidationResult($"Default provider '{DefaultProvider}' is not configured in Providers.", [nameof(DefaultProvider)]));

        foreach (var (name, agent) in Agents)
        {
            var provider = agent.Provider ?? DefaultProvider;
            if (!Providers.ContainsKey(provider))
                results.Add(new ValidationResult($"Agent '{name}' references provider '{provider}' which is not configured.", [nameof(Agents)]));
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
