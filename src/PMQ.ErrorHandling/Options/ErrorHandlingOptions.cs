namespace PMQ.ErrorHandling.Options
{
    /// <summary>
    /// Configuration options for the PMQ error handling system.
    /// </summary>
    /// <remarks>
    /// This class provides centralized configuration for error handling behavior,
    /// including localization, exception details exposure, and custom message mappings.
    /// </remarks>
    public class ErrorHandlingOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to include exception details 
        /// in error responses.
        /// </summary>
        /// <value>
        /// <c>true</c> to include exception details; otherwise <c>false</c>.
        /// Default is <c>true</c>.
        /// </value>
        /// <remarks>
        /// When set to <c>true</c>, the exception message will be included in the
        /// error response's Detail property. This is useful for development but should
        /// be disabled in production to avoid exposing sensitive information.
        /// </remarks>
        public bool IncludeExceptionDetails { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include the trace ID 
        /// in error responses.
        /// </summary>
        /// <value>
        /// <c>true</c> to include trace ID; otherwise <c>false</c>.
        /// Default is <c>true</c>.
        /// </value>
        /// <remarks>
        /// The trace ID helps correlate errors with application logs for debugging.
        /// </remarks>
        public bool IncludeTraceId { get; set; } = true;

        /// <summary>
        /// Gets or sets the culture for error message localization.
        /// </summary>
        /// <value>
        /// The culture code (e.g., "en-US", "pt-BR").
        /// Default is "en-US".
        /// </value>
        /// <remarks>
        /// The system supports:
        /// <list type="bullet">
        /// <item><description>English ("en-US" or any non-Portuguese culture)</description></item>
        /// <item><description>Portuguese Brazil ("pt-BR" or any culture starting with "pt")</description></item>
        /// </list>
        /// </remarks>
        public string Culture { get; set; } = "en-US";

        /// <summary>
        /// Gets or sets custom error message mappings to override default localized messages.
        /// </summary>
        /// <value>
        /// A dictionary where keys are error message identifiers and values are custom messages.
        /// Default is an empty dictionary.
        /// </value>
        /// <remarks>
        /// Use this to override default messages for specific error keys. Example keys include:
        /// <list type="bullet">
        /// <item><description>InternalServerError</description></item>
        /// <item><description>ValidationError</description></item>
        /// <item><description>NotFound</description></item>
        /// <item><description>AccessDenied</description></item>
        /// <item><description>InconsistentState</description></item>
        /// <item><description>BusinessRule</description></item>
        /// </list>
        /// </remarks>
        public Dictionary<string, string> CustomMessages { get; set; } = [];
    }
}
