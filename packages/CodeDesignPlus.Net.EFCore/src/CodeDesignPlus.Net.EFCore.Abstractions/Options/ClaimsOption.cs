using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Options;

/// <summary>
/// Claims available to obtain user information.
/// </summary>
public class ClaimsOption
{
    /// <summary>
    /// Gets or sets the claim to get the user's name.
    /// </summary>
    [Required]
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the claim to get the user's ID.
    /// </summary>
    [Required]
    public string IdUser { get; set; }

    /// <summary>
    /// Gets or sets the claim to get the user's email.
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the claim to get the user's role.
    /// </summary>
    [Required]
    public string Role { get; set; }
}