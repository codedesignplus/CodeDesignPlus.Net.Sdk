using System;

namespace CodeDesignPlus.Net.Vault.Abstractions.Options;

public class RabbitMQ
{
    public bool Enable { get; set; } = true;
    public string RoleSufix { get; set; } = "rabbitmq-role";
    public string SufixMoundPoint { get; set; } = "rabbitmq";
}
