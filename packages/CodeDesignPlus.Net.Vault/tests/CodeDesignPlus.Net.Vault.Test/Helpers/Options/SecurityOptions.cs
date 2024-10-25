using System;

namespace CodeDesignPlus.Net.Vault.Test.Helpers.Options;

public class SecurityOptions
{
    public string? ClientId { get; set; }
    public string[] ValidAudiences { get; set; } = [];
}
