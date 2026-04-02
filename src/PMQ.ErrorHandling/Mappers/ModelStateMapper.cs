using Microsoft.AspNetCore.Mvc.ModelBinding;
using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Mappers;

public static class ModelStateMapper
{
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