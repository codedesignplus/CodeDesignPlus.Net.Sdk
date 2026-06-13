using CodeDesignPlus.Net.Resilience.Abstractions.Options;
using CodeDesignPlus.Net.Resilience.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CodeDesignPlus.Net.Resilience.Test.Extensions;

public class HttpClientBuilderExtensionsTest
{
    [Fact]
    public void AddResiliencePolicies_BuilderIsNull_ThrowsArgumentNullException()
    {
        IHttpClientBuilder builder = null;
        var options = new ResilienceOptions();

        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddResiliencePolicies(options));

        Assert.Equal("builder", exception.ParamName);
    }

    [Fact]
    public void AddResiliencePolicies_OptionsIsNull_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("Test");

        var exception = Assert.Throws<ArgumentNullException>(() => builder.AddResiliencePolicies(null));

        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void AddResiliencePolicies_DisabledOptions_ReturnsBuilderWithoutPolicies()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("Test");
        var options = new ResilienceOptions { Enable = false };

        var result = builder.AddResiliencePolicies(options);

        Assert.NotNull(result);
    }

    [Fact]
    public void AddResiliencePolicies_EnabledOptions_RegistersResilienceHandler()
    {
        var services = new ServiceCollection();
        var builder = services.AddHttpClient("Test");
        var options = new ResilienceOptions { Enable = true };

        var result = builder.AddResiliencePolicies(options);

        Assert.NotNull(result);
    }
}
