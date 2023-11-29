using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Security.Abstractions.Options;

/// <summary>
/// Options to setting of the Security
/// </summary>
public class SecurityOptions 
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Security";
    public Uri Authority { get; set; }
}
