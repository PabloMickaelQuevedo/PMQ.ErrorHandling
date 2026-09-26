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
        /// Default is <c>false</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// When set to <c>true</c>, the exception message is copied into the error response's
        /// Detail property. Exception messages routinely carry connection strings, SQL, file
        /// paths and internal identifiers, so this is opt-in: turn it on for development only,
        /// typically with <c>builder.Environment.IsDevelopment()</c>.
        /// </para>
        /// <para>
        /// Up to 1.1.4 the default was <c>true</c>, which exposed exception messages in
        /// production for any application that did not set this explicitly.
        /// </para>
        /// </remarks>
        public bool IncludeExceptionDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the trace ID 
        /// in error responses.
        /// </summary>
        /// <value>
        /// <c>true</c> to include trace ID; otherwise <c>false</c>.
        /// Default is <c>true</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// The trace ID helps correlate errors with application logs for debugging. It comes from
        /// the current trace — <c>Activity.Current</c>, falling back to the request's trace
        /// identifier — on every error response: exceptions, notifications, invalid model state
        /// and status code pages.
        /// </para>
        /// <para>
        /// Up to 1.1.x this option was not read anywhere and the trace ID was always included.
        /// </para>
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
