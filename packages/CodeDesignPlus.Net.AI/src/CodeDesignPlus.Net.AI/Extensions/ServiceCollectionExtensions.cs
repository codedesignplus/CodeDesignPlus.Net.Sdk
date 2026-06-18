using CodeDesignPlus.Net.AI.Abstractions.DocumentIntelligence;
using CodeDesignPlus.Net.AI.DocumentIntelligence;

namespace CodeDesignPlus.Net.AI.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers AI services based on configuration.
    /// - If no "AI" section exists, registration is skipped
    /// - If Providers are configured, chat services are registered
    /// - If DocumentIntelligence is configured, document services are registered
    /// Both features are independent and can be used separately or together.
    /// </summary>
    public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(AIOptions.Section);

        if (!section.Exists())
            return services; // Silent skip when AI not configured

        services
            .AddOptions<AIOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        // Get options to determine what to register
        var aiOptions = section.Get<AIOptions>();

        // Register chat services only if providers are configured
        if (aiOptions?.Providers?.Count > 0)
        {
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IAIProvider, ClaudeProvider>();
            services.AddScoped<IAIProvider, OpenAIProvider>();
            services.AddScoped<IAIProvider, AzureOpenAIProvider>();
        }

        // Document Intelligence — only register if configured
        if (aiOptions?.DocumentIntelligence is not null)
        {
            services.AddScoped<IDocumentIntelligenceService, DocumentIntelligenceService>();
            services.AddScoped<IDocumentIntelligenceProvider, AzureDocumentIntelligenceProvider>();
        }

        return services;
    }
}
