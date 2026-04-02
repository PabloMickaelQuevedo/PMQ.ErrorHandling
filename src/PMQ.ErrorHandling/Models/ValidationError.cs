namespace PMQ.ErrorHandling.Models
{
    /// <summary>
    /// Represents a validation error that occurred during model validation.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Gets or sets the error message describing the validation failure.
        /// </summary>
        public string Message { get; set; } = default!;

        /// <summary>
        /// Gets or sets the field name that failed validation, if applicable.
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Gets or sets an optional error code for the validation failure.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="message">The error message describing the validation failure.</param>
        /// <param name="field">The name of the field that failed validation, if applicable.</param>
        /// <param name="code">An optional error code for the validation failure.</param>
        public ValidationError(
            string message,
            string? field = null,
            string? code = null)
        {
            Message = message;
            Field = field;
            Code = code;
        }
    }
}
