using CodeDesignPlus.Net.AI.Abstractions.Models;

namespace CodeDesignPlus.Net.AI.Services;

public class AIService(
    IOptions<AIOptions> options,
    IEnumerable<IAIProvider> providers,
    ILogger<AIService> logger
) : IAIService
{
    private readonly AIOptions options = options.Value;

    public async Task<ChatResponse> ChatAsync(string agentName, string userMessage, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage> {
            new(ChatRole.User, userMessage)
        };

        return await ChatAsync(agentName, messages, cancellationToken);
    }

    public async Task<ChatResponse> ChatAsync(string agentName, IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        if (!options.Agents.TryGetValue(agentName, out var agentOptions))
            throw new AIException($"Agent '{agentName}' is not configured. Available agents: {string.Join(", ", options.Agents.Keys)}");

        var providerName = agentOptions.Provider ?? options.DefaultProvider;

        if (!options.Providers.TryGetValue(providerName, out var providerOptions))
            throw new AIException($"Provider '{providerName}' is not configured for agent '{agentName}'.");

        var provider = providers.FirstOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new AIException($"No implementation found for provider '{providerName}'. Registered providers: {string.Join(", ", providers.Select(p => p.Name))}");

        var fullMessages = new List<ChatMessage>();

        if (!string.IsNullOrEmpty(agentOptions.SystemPrompt))
            fullMessages.Add(new ChatMessage(ChatRole.System, agentOptions.SystemPrompt));

        fullMessages.AddRange(messages);

        logger.LogDebug("Sending chat request to agent '{Agent}' via provider '{Provider}' with {Count} messages", agentName, providerName, fullMessages.Count);

        var response = await provider.ChatAsync(fullMessages, agentOptions, providerOptions, cancellationToken);

        if (response.Success)
            logger.LogInformation("Agent '{Agent}' responded successfully. Tokens: {Total} (prompt: {Prompt}, completion: {Completion})", agentName, response.TotalTokens, response.PromptTokens, response.CompletionTokens);
        else
            logger.LogWarning("Agent '{Agent}' failed: {Error}", agentName, response.Error);

        return response;
    }
}
