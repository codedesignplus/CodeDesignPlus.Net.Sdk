using CodeDesignPlus.Net.AI.Abstractions.Models;
using CodeDesignPlus.Net.AI.Abstractions.Options;

namespace CodeDesignPlus.Net.AI.Abstractions.Providers;

/// <summary>
/// Defines the contract for an AI/LLM provider implementation.
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Gets the unique name of the provider (e.g., "Claude", "OpenAI", "AzureOpenAI").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Sends a chat request to the AI provider and returns the response.
    /// </summary>
    /// <param name="messages">The full conversation including system prompt and user messages.</param>
    /// <param name="agentOptions">Agent-level configuration (temperature, max tokens, model override).</param>
    /// <param name="providerOptions">Provider-level configuration (API key, endpoint).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The provider's chat response.</returns>
    Task<ChatResponse> ChatAsync(
        IReadOnlyList<ChatMessage> messages,
        AgentOptions agentOptions,
        ProviderOptions providerOptions,
        CancellationToken cancellationToken = default
    );
}
