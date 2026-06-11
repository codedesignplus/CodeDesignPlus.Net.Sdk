namespace CodeDesignPlus.Net.AI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(AIOptions.Section);

        if (!section.Exists())
            throw new AIException($"The section '{AIOptions.Section}' is required in configuration.");

        services
            .AddOptions<AIOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddScoped<IAIService, AIService>();

        services.AddScoped<IAIProvider, ClaudeProvider>();
        services.AddScoped<IAIProvider, OpenAIProvider>();
        services.AddScoped<IAIProvider, AzureOpenAIProvider>();

        return services;
    }
}
