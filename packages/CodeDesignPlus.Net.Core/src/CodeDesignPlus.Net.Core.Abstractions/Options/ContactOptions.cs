namespace CodeDesignPlus.Net.Core.Abstractions.Options;

/// <summary>
/// Represents a contact information.
/// </summary>
public class Contact
{
    /// <summary>
    /// Gets or sets the name of the contact.
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the email of the contact.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
