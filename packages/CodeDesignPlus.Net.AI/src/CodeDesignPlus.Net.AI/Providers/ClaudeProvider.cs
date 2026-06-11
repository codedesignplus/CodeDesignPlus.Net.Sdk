using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using CodeDesignPlus.Net.AI.Abstractions.Models;
using CodeDesignPlus.Net.AI.Abstractions.Options;
using CodeDesignPlus.Net.AI.Abstractions.Providers;

namespace CodeDesignPlus.Net.AI.Providers;

public class ClaudeProvider : IAIProvider
{
    public string Name => "Claude";

    public async Task<ChatResponse> ChatAsync(
        IReadOnlyList<ChatMessage> messages,
        AgentOptions agentOptions,
        ProviderOptions providerOptions,
        CancellationToken cancellationToken = default)
    {
        var client = new AnthropicClient(providerOptions.ApiKey);

        var model = agentOptions.Model ?? providerOptions.Model ?? "claude-sonnet-4-5-20250514";

        var systemMessage = messages.FirstOrDefault(m => m.Role == ChatRole.System)?.Content
            ?? agentOptions.SystemPrompt;

        var anthropicMessages = messages
            .Where(m => m.Role != ChatRole.System)
            .Select(m => new Message
            {
                Role = m.Role == ChatRole.User ? RoleType.User : RoleType.Assistant,
                Content = [new TextContent { Text = m.Content }]
            })
            .ToList();

        var request = new MessageParameters
        {
            Model = model,
            MaxTokens = agentOptions.MaxTokens,
            Temperature = (decimal)agentOptions.Temperature,
            SystemMessage = systemMessage ?? string.Empty,
            Messages = anthropicMessages
        };

        try
        {
            var response = await client.Messages.GetClaudeMessageAsync(request, ctx: cancellationToken);

            var content = response.Content?.FirstOrDefault()?.ToString() ?? string.Empty;

            return new ChatResponse
            {
                Content = content,
                Success = true,
                Model = model,
                PromptTokens = response.Usage?.InputTokens ?? 0,
                CompletionTokens = response.Usage?.OutputTokens ?? 0,
                TotalTokens = (response.Usage?.InputTokens ?? 0) + (response.Usage?.OutputTokens ?? 0)
            };
        }
        catch (Exception ex)
        {
            return new ChatResponse
            {
                Content = string.Empty,
                Success = false,
                Error = ex.Message,
                Model = model
            };
        }
    }
}
