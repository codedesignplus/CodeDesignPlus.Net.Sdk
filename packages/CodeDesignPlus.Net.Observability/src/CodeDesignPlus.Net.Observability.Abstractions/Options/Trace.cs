namespace CodeDesignPlus.Net.Observability.Abstractions.Options;

public class Trace
{
    public bool Enable { get; set; }
    public bool AspNetCore { get; set; }
    public bool CodeDesignPlusSdk { get; set; }
    public bool Redis { get; set; }
    public bool Kafka { get; set; }
    public bool SqlClient { get; set; }
    public bool GrpcClient { get; set; }
}
