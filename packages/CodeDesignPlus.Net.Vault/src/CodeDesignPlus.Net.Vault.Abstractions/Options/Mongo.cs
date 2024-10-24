using System;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

public class Mongo
{
    public bool Enable { get; set; } = true;
    public string RoleSufix { get; set; } = "mongo-role";
    public string SufixMoundPoint { get; set; } = "database";
    public string TemplateConnectionString = "mongodb://{0}:{1}@mongo:27017";
}
