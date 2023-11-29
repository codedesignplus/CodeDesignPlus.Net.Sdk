using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Options;

/// <summary>
/// Options to setting of the EFCore
/// </summary>
public class EFCoreOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "EFCore";

    /// <summary>
    /// Gets or sets the claims identity 
    /// </summary>
    [Required]
    public ClaimsOption ClaimsIdentity { get; set; }
}
