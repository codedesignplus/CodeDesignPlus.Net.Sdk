using System.ClientModel;
using Azure.AI.OpenAI;
using CodeDesignPlus.Net.AI.Abstractions.Options;
using CodeDesignPlus.Net.AI.Abstractions.Providers;
using OpenAI.Chat;
using AIChatMessage = CodeDesignPlus.Net.AI.Abstractions.Models.ChatMessage;
using AIChatRole = CodeDesignPlus.Net.AI.Abstractions.Models.ChatRole;
using AIChatResponse = CodeDesignPlus.Net.AI.Abstractions.Models.ChatResponse;

namespace CodeDesignPlus.Net.AI.Providers;

public class AzureOpenAIProvider : IAIProvider
{
    public string Name => "AzureOpenAI";

    public async Task<AIChatResponse> ChatAsync(
        IReadOnlyList<AIChatMessage> messages,
        AgentOptions agentOptions,
        ProviderOptions providerOptions,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(providerOptions.Endpoint))
            return new AIChatResponse { Content = string.Empty, Success = false, Error = "Azure OpenAI endpoint is required." };

        var client = new AzureOpenAIClient(
            new Uri(providerOptions.Endpoint),
            new ApiKeyCredential(providerOptions.ApiKey)
        );

        var model = agentOptions.Model ?? providerOptions.Model ?? "gpt-4o";
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
