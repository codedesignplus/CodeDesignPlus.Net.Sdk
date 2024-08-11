﻿
namespace CodeDesignPlus.Net.xUnit.Helpers;

public static class DataAnnotationsExtensions
{
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
