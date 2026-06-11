using CodeDesignPlus.Net.AI.Abstractions.Models;

namespace CodeDesignPlus.Net.AI.Abstractions;

/// <summary>
/// Provides AI chat capabilities through configured agents and providers.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Sends a single user message to the specified agent and returns the AI response.
    /// </summary>
    /// <param name="agentName">The name of the configured agent to use.</param>
    /// <param name="userMessage">The user message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI chat response containing generated content and token usage.</returns>
    Task<ChatResponse> ChatAsync(string agentName, string userMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a multi-turn conversation to the specified agent and returns the AI response.
    /// </summary>
    /// <param name="agentName">The name of the configured agent to use.</param>
    /// <param name="messages">The conversation history including user and assistant messages.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI chat response containing generated content and token usage.</returns>
    Task<ChatResponse> ChatAsync(string agentName, IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken = default);
}
