using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public class ValidationHelper
{
    /// <summary>
    /// Model validation using ValidationContext
    /// </summary>
    /// <param name="obj">Model object to validate</param>
    /// <exception cref="ArgumentException">When one or more validation errors found</exception>
    internal static void ModelValidation(object obj)
    {
        ValidationContext validationContext = new(obj);
        List<ValidationResult> validationResults = new();

        bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
        if (!isValid)
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
    }
}
