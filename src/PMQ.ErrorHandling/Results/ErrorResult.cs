using Microsoft.AspNetCore.Mvc;
using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Results
{
    public class ErrorResult : ObjectResult
    {
        public ErrorResult(ErrorDetails errorDetails)
            : base(errorDetails)
        {
            StatusCode = errorDetails.Status ?? 400;
        }

        public static ErrorResult From (string message, int statusCode)
        {
            return new ErrorResult(new ErrorDetails
            {
                Title = message,
                Status = statusCode
            });
        }
    }
}
