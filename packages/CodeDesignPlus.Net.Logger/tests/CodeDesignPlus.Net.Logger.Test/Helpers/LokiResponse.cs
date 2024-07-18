using CodeDesignPlus.Net.Serializers;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Logger.Test.Helpers;

public class Stream
{
    [JsonProperty("detected_level")]
    public string? SetectedLevel { get; set; }
    public string? Sxporter { get; set; }
    public string? Sob { get; set; }
    public string? Sevel { get; set; }
    [JsonProperty("service_name")]
    public string? ServiceName { get; set; }
}

public class Value
{
    public Stream Stream { get; set; } = new Stream();
    public List<List<string>> Values { get; set; } = [];
}

public class Data
{
    public string? ResultType { get; set; }
    public List<Value> Result { get; set; } = [];
}

public class RootObject
{
    public string? Status { get; set; }
    public Data Data { get; set; } = new Data();
}


public class LogEntry
{
    public string? Body { get; set; }

    public string? Severity { get; set; }

    public LogAttributes? Attributes { get; set; }

    public LogResources? Resources { get; set; }

    [JsonProperty("instrumentation_scope")]
    public InstrumentationScope? InstrumentationScope { get; set; }
}

public class LogAttributes
{
    public string? AppName { get; set; }

    public string? EnvironmentUserName { get; set; }

    public string? MachineName { get; set; }

    public int ProcessId { get; set; }

    public string? ProcessName { get; set; }

    public int ThreadId { get; set; }

    public string? ThreadName { get; set; }

    [JsonProperty("message_template.hash.md5")]
    public string? MessageTemplateHashMd5 { get; set; }

    [JsonProperty("message_template.text")]
    public string? MessageTemplateText { get; set; }
}

public class LogResources
{
    [JsonProperty("service.business")]
    public string? ServiceBusiness { get; set; }

    [JsonProperty("service.contact.email")]
    public string? ServiceContactEmail { get; set; }

    [JsonProperty("service.contact.name")]
    public string? ServiceContactName { get; set; }

    [JsonProperty("service.description")]
    public string? ServiceDescription { get; set; }

    [JsonProperty("service.name")]
    public string? ServiceName { get; set; }

    [JsonProperty("service.version")]
    public string? ServiceVersion { get; set; }
}

public class InstrumentationScope
{
    public string? Name { get; set; }
}