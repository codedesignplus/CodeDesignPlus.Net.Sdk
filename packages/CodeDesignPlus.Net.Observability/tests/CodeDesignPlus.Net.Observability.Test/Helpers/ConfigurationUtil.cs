namespace CodeDesignPlus.Net.Observability.Test.Helpers;

public static class ConfigurationUtil
{
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
