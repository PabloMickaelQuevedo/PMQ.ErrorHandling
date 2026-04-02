using Microsoft.AspNetCore.Mvc;

namespace PMQ.ErrorHandling.Models
{
    /// <summary>
    /// Represents standardized error details for API responses.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ProblemDetails"/> with additional error information
    /// including validation errors and trace identifiers for debugging.
    /// </remarks>
    public class ErrorDetails : ProblemDetails
    {
        /// <summary>
        /// Gets or sets a collection of validation errors that occurred during model validation.
        /// </summary>
        public IEnumerable<ValidationError>? Errors { get; set; }

        /// <summary>
        /// Gets or sets the trace identifier for correlating this error with log entries.
        /// </summary>
        public string? TraceId { get; set; }
    }
}
