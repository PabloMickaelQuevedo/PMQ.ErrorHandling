using Microsoft.AspNetCore.Mvc.ModelBinding;
using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Mappers;

/// <summary>
/// Provides extension methods for mapping ASP.NET Core ModelState errors to validation errors.
/// </summary>
public static class ModelStateMapper
{
    /// <summary>
    /// Converts ModelState validation errors to a collection of validation errors.
    /// </summary>
    /// <param name="modelState">The model state dictionary containing validation errors.</param>
    /// <returns>A collection of <see cref="ValidationError"/> objects representing model validation failures.</returns>
    public static IEnumerable<ValidationError> ToValidationErrors(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(kvp =>
                kvp.Value!.Errors.Select(error =>
                    new ValidationError(
                        message: error.ErrorMessage,
                        field: kvp.Key
                    )
                )
            );
    }
}