﻿namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

/// <summary>
/// Options for configuring Azure File Storage.
/// </summary>
public class AzureFileOptions : AzureOptions, IValidatableObject
{
    /// <summary>
    /// Gets the type of provider.
    /// </summary>
    public static TypeProviders TypeProvider { get => TypeProviders.AzureFileProvider; }

    /// <summary>
    /// Validates the options.
    /// </summary>
    /// <param name="validationContext">The context of the validation.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return Validate();
    }
}