namespace CodeDesignPlus.Net.EFCore.Abstractions.Options;

/// <summary>
/// Options for configuring EFCore.
/// </summary>
public class EFCoreOptions
{
    /// <summary>
    /// Name of the section used in the appsettings.
    /// </summary>
    public static readonly string Section = "EFCore";

    /// <summary>
    /// Gets or sets the connection string to the database.
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    /// Gets or sets the connection string to the database.
    /// </summary>
    public bool RegisterRepositories { get; set; } = true;
}