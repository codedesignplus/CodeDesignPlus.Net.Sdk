namespace CodeDesignPlus.Net.Observability.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly ObservabilityOptions ObservabilityOptions = new()
    {
        Enable = true,
        Name = nameof(Abstractions.Options.ObservabilityOptions.Name),
        Email = $"{nameof(Abstractions.Options.ObservabilityOptions.Name)}@codedesignplus.com"
    };
}
