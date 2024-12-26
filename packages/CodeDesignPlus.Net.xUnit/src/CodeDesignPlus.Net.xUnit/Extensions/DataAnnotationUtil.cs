namespace CodeDesignPlus.Net.xUnit.Extensions;

/// <summary>
/// Provides extension methods for data annotations validation.
/// </summary>
public static class DataAnnotationsExtensions
{
    /// <summary>
    /// Validates the specified data object using data annotations.
    /// </summary>
    /// <typeparam name="T">The type of the data object to validate.</typeparam>
    /// <param name="data">The data object to validate.</param>
    /// <returns>A list of <see cref="ValidationResult"/> containing the validation results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the data object is null.</exception>
    public static IList<ValidationResult> Validate<T>(this T data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var results = new List<ValidationResult>();

        var validationContext = new ValidationContext(data, null, null);

        Validator.TryValidateObject(data, validationContext, results, true);

        if (data is IValidatableObject @object)
            results.AddRange(@object.Validate(validationContext));

        return results;
    }
}