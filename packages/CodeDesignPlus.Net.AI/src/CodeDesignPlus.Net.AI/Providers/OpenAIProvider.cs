using System.ClientModel;
using CodeDesignPlus.Net.AI.Abstractions.Options;
using CodeDesignPlus.Net.AI.Abstractions.Providers;
using OpenAI;
using OpenAI.Chat;
using AIChatMessage = CodeDesignPlus.Net.AI.Abstractions.Models.ChatMessage;
using AIChatRole = CodeDesignPlus.Net.AI.Abstractions.Models.ChatRole;
using AIChatResponse = CodeDesignPlus.Net.AI.Abstractions.Models.ChatResponse;

namespace CodeDesignPlus.Net.AI.Providers;

public class OpenAIProvider : IAIProvider
{
    public string Name => "OpenAI";

    public async Task<AIChatResponse> ChatAsync(
        IReadOnlyList<AIChatMessage> messages,
        AgentOptions agentOptions,
        ProviderOptions providerOptions,
        CancellationToken cancellationToken = default)
    {
        var model = agentOptions.Model ?? providerOptions.Model ?? "gpt-4o";

        var clientOptions = new OpenAIClientOptions();
        if (!string.IsNullOrEmpty(providerOptions.Endpoint))
            clientOptions.Endpoint = new Uri(providerOptions.Endpoint);

        var client = new OpenAIClient(new ApiKeyCredential(providerOptions.ApiKey), clientOptions);
        var chatClient = client.GetChatClient(model);

        var openAiMessages = messages.Select<AIChatMessage, ChatMessage>(m => m.Role switch
        {
            AIChatRole.System => new SystemChatMessage(m.Content),
            AIChatRole.User => new UserChatMessage(m.Content),
            AIChatRole.Assistant => new AssistantChatMessage(m.Content),
            _ => new UserChatMessage(m.Content)
        }).ToList();

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = agentOptions.MaxTokens,
            Temperature = (float)agentOptions.Temperature,
            TopP = (float)agentOptions.TopP,
        };

        try
        {
            var response = await chatClient.CompleteChatAsync(openAiMessages, options, cancellationToken);
            var result = response.Value;

            return new AIChatResponse
            {
                Content = result.Content?.FirstOrDefault()?.Text ?? string.Empty,
                Success = true,
                Model = model,
                PromptTokens = result.Usage?.InputTokenCount ?? 0,
                CompletionTokens = result.Usage?.OutputTokenCount ?? 0,
                TotalTokens = result.Usage?.TotalTokenCount ?? 0
            };
        }
        catch (Exception ex)
        {
            return new AIChatResponse
            {
                Content = string.Empty,
                Success = false,
                Error = ex.Message,
                Model = model
            };
        }
    }
}
