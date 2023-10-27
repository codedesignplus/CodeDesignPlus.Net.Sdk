namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Provides properties for auditing trail purposes.
/// </summary>
/// <typeparam name="TUserKey">Type of the user identifier.</typeparam>
public interface IAuditTrail<TUserKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    TUserKey IdUserCreator { get; set; }
    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    DateTime DateCreated { get; set; }
}