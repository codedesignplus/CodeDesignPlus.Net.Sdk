# CodeDesignPlus.Net.AI

[![NuGet](https://img.shields.io/nuget/v/CodeDesignPlus.Net.AI.svg)](https://www.nuget.org/packages/CodeDesignPlus.Net.AI/)

## Overview

`CodeDesignPlus.Net.AI` provides a unified interface for integrating AI/LLM providers (Claude, OpenAI, Azure OpenAI) into .NET microservices. It supports configurable agents with system prompts, model selection, and provider routing via `appsettings.json`.

## Features

- **Multi-Provider Support**: Claude (Anthropic), OpenAI, and Azure OpenAI out of the box
- **Agent Configuration**: Define named agents with system prompts, temperature, max tokens, and provider selection
- **Provider Abstraction**: `IAIProvider` interface allows custom provider implementations
- **Options Pattern**: Standard .NET configuration via `appsettings.json` with validation
- **Dependency Injection**: `services.AddAI(configuration)` registers all services

## Installation

```bash
dotnet add package CodeDesignPlus.Net.AI
```

## Configuration

```json
{
  "AI": {
    "DefaultProvider": "Claude",
    "Providers": {
      "Claude": {
        "ApiKey": "sk-ant-...",
        "Model": "claude-sonnet-4-5-20250514"
      },
      "OpenAI": {
        "ApiKey": "sk-...",
        "Model": "gpt-4o"
      },
      "AzureOpenAI": {
        "ApiKey": "...",
        "Model": "gpt-4o",
        "Endpoint": "https://your-resource.openai.azure.com/"
      }
    },
    "Agents": {
      "EmailDesigner": {
        "Provider": "Claude",
        "SystemPrompt": "You are an expert HTML email template designer...",
        "MaxTokens": 4096,
        "Temperature": 0.7
      }
    }
  }
}
```

## Usage

### Registration

```csharp
// In Startup.cs or Program.cs
services.AddAI(configuration);
```

### Consuming the Service

```csharp
public class MyCommandHandler(IAIService ai) : IRequestHandler<MyCommand>
{
    public async Task Handle(MyCommand request, CancellationToken cancellationToken)
    {
        var response = await ai.ChatAsync("EmailDesigner", "Create a welcome email template", cancellationToken);

        if (response.Success)
        {
            // Use response.Content (the AI-generated text/HTML)
        }
    }
}
```

### Multi-turn Conversations

```csharp
var messages = new List<ChatMessage>
{
    new(ChatRole.User, "Create a purchase receipt email"),
    new(ChatRole.Assistant, "<html>...</html>"),
    new(ChatRole.User, "Change the header color to blue"),
};

var response = await ai.ChatAsync("EmailDesigner", messages, cancellationToken);
```

## Architecture

```
IAIService (main entry point)
    ↓
AIService (orchestrator - resolves agent config + provider)
    ├── ClaudeProvider (Anthropic SDK)
    ├── OpenAIProvider (OpenAI SDK)
    └── AzureOpenAIProvider (Azure.AI.OpenAI SDK)
```

## Documentation

For more information, visit [https://doc.codedesignplus.com](https://doc.codedesignplus.com)

## License

This project is licensed under the GNU Lesser General Public License v3.0. See [LICENSE.md](LICENSE.md) for details.
