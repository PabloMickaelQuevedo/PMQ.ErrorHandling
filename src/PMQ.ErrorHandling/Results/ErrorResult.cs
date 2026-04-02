using Microsoft.AspNetCore.Mvc;
using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Results
{
    /// <summary>
    /// Represents an action result that returns error details with an appropriate HTTP status code.
    /// </summary>
    public class ErrorResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResult"/> class with error details.
        /// </summary>
        /// <param name="errorDetails">The error details to include in the response.</param>
        public ErrorResult(ErrorDetails errorDetails)
            : base(errorDetails)
        {
            StatusCode = errorDetails.Status ?? 400;
        }

        /// <summary>
        /// Creates a new <see cref="ErrorResult"/> from a message and status code.
        /// </summary>
        /// <param name="message">The error message to include in the response.</param>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        /// <returns>A new instance of <see cref="ErrorResult"/> with the specified message and status code.</returns>
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
