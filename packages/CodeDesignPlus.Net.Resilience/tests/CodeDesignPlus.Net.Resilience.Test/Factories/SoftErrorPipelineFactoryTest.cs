using CodeDesignPlus.Net.Resilience.Abstractions;
using CodeDesignPlus.Net.Resilience.Abstractions.Options;
using CodeDesignPlus.Net.Resilience.Factories;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Xunit;

namespace CodeDesignPlus.Net.Resilience.Test.Factories;

public class SoftErrorPipelineFactoryTest
{
    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });

    [Fact]
    public void Create_OptionsIsNull_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            SoftErrorPipelineFactory.Create(null, _loggerFactory));

        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void Create_LoggerFactoryIsNull_ThrowsArgumentNullException()
    {
        var options = new ResilienceOptions();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            SoftErrorPipelineFactory.Create(options, null));

        Assert.Equal("loggerFactory", exception.ParamName);
    }

    [Fact]
    public void Create_Disabled_ReturnsEmptyPipeline()
    {
        var options = new ResilienceOptions { Enable = false };

        var pipeline = SoftErrorPipelineFactory.Create(options, _loggerFactory);

        Assert.Same(ResiliencePipeline.Empty, pipeline);
    }

    [Fact]
    public void Create_SoftErrorsDisabled_ReturnsEmptyPipeline()
    {
        var options = new ResilienceOptions { Enable = true, RetryOnSoftErrors = false };

        var pipeline = SoftErrorPipelineFactory.Create(options, _loggerFactory);

        Assert.Same(ResiliencePipeline.Empty, pipeline);
    }

    [Fact]
    public void Create_Enabled_ReturnsPipeline()
    {
        var options = new ResilienceOptions
        {
            Enable = true,
            RetryOnSoftErrors = true,
            MaxSoftErrorRetryAttempts = 3,
            SoftErrorRetryBaseDelaySeconds = 1.0,
            SoftErrorMaxDelaySeconds = 5.0
        };

        var pipeline = SoftErrorPipelineFactory.Create(options, _loggerFactory);

        Assert.NotNull(pipeline);
        Assert.NotSame(ResiliencePipeline.Empty, pipeline);
    }

    [Fact]
    public async Task Create_Pipeline_RetriesOnSoftErrorException()
    {
        var options = new ResilienceOptions
        {
            Enable = true,
            RetryOnSoftErrors = true,
            MaxSoftErrorRetryAttempts = 2,
            SoftErrorRetryBaseDelaySeconds = 0.01,
            SoftErrorMaxDelaySeconds = 0.02
        };

        var pipeline = SoftErrorPipelineFactory.Create(options, _loggerFactory);
        var attempts = 0;

        await Assert.ThrowsAsync<ResilienceSoftErrorException>(async () =>
        {
            await pipeline.ExecuteAsync(async ct =>
            {
                attempts++;
                throw new ResilienceSoftErrorException("test error", "response body");
            });
        });

        Assert.Equal(3, attempts);
    }
}
