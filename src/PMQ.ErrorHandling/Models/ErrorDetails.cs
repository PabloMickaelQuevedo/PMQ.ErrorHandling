using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace PMQ.ErrorHandling.Models
{
    /// <summary>
    /// Represents standardized error details for API responses.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ProblemDetails"/> (RFC 9457) with two extension members:
    /// the individual errors that caused the response and a trace identifier to correlate it with
    /// logs. Like the members inherited from <see cref="ProblemDetails"/>, they are omitted from
    /// the JSON when empty rather than written as <c>null</c>.
    /// </remarks>
    public class ErrorDetails : ProblemDetails
    {
        /// <summary>
        /// Gets or sets a collection of validation errors that occurred during model validation.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ValidationError>? Errors { get; set; }

        /// <summary>
        /// Gets or sets the trace identifier for correlating this error with log entries.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceId { get; set; }
    }
}
