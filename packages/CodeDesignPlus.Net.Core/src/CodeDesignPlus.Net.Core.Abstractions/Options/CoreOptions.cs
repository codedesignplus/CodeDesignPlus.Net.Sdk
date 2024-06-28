using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Core.Abstractions.Options;

/// <summary>
/// Options to setting of the Core
/// </summary>
public class CoreOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Core";

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [Required]
    public string? AppName { get; set; }

    [Required]
    public string? Version { get; set; }

    public string? Description { get; set; }

    public ContactOptions Contact { get; set; } = new();
}

public class ContactOptions
{
    public string? Name { get; set; }

    public string? Email { get; set; }
}
