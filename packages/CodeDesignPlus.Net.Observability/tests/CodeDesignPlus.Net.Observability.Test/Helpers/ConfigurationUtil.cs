using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.Observability.Test.Helpers;

public static class ConfigurationUtil
{

    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "opentelemetry-test",
        Version = "1.0.0",
        Description = "Test application for the OpenTelemetry library.",
        Business = "CodeDesignPlus",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "codedesignplus@outlook.com"
        }
    };

    public static readonly ObservabilityOptions ObservabilityOptions = new()
    {
        Enable = true,
        ServerOtel = new Uri("http://localhost:4317"),
        Metrics = new Metrics()
        {
            Enable = true,
            AspNetCore = true
        },
        Trace = new Trace()
        {
            Enable = true,
            AspNetCore = true,
            CodeDesignPlusSdk = true,
            Redis = true,
            Kafka = true,
            SqlClient = true,
            GrpcClient = true
        }
    };
}
