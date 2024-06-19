﻿namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Provides properties for auditing trail purposes.
/// </summary>
public interface IAuditTrail
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    Guid CreateBy { get; set; }
    /// <summary>
    /// Gets or sets the date when the record was created.
    /// </summary>
    DateTime CreatedAt { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user who updated the record.
    /// </summary>
    Guid? UpdateBy { get; set; }
    /// <summary>
    /// Gets or sets the date when the record was updated.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}