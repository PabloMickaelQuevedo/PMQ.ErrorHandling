using Microsoft.Extensions.Options;
using PMQ.ErrorHandling.Constants;
using PMQ.ErrorHandling.Interfaces;
using PMQ.ErrorHandling.Options;

namespace PMQ.ErrorHandling.Localization
{
    /// <summary>
    /// Default implementation of <see cref="IErrorLocalizer"/> with support for multiple cultures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This localizer provides error messages based on the configured culture.
    /// It supports:
    /// <list type="bullet">
    /// <item><description>English (default for "en-US" and non-Portuguese cultures)</description></item>
    /// <item><description>Portuguese (Brazil) for "pt-BR" and cultures starting with "pt")</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Custom messages can override default localizations through the <see cref="ErrorHandlingOptions.CustomMessages"/> dictionary.
    /// </para>
    /// </remarks>
    public class DefaultErrorLocalizer : IErrorLocalizer
    {
        private readonly ErrorHandlingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultErrorLocalizer"/> class.
        /// </summary>
        /// <param name="options">The error handling options containing culture and custom messages configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
        public DefaultErrorLocalizer(IOptions<ErrorHandlingOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets a localized error message by its key.
        /// </summary>
        /// <param name="key">The error message key to retrieve.</param>
        /// <returns>
        /// The localized message corresponding to the key, or the key itself if not found.
        /// Custom messages take precedence over default messages.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method follows the following priority order:
        /// <list type="number">
        /// <item><description>Custom messages configured in <see cref="ErrorHandlingOptions.CustomMessages"/></description></item>
        /// <item><description>Language-specific default messages based on the configured culture</description></item>
        /// <item><description>The key itself as a fallback</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public string Get(string key)
        {
            // Check custom messages first
            if (_options.CustomMessages.TryGetValue(key, out var customMessage))
            {
                return customMessage;
            }

            // Get message based on configured culture
            return _options.Culture?.StartsWith("pt") ?? false
                ? GetPortugueseBRMessage(key)
                : GetEnglishMessage(key);
        }

        /// <summary>
        /// Gets the English version of an error message.
        /// </summary>
        /// <param name="key">The message key.</param>
        /// <returns>The English error message corresponding to the key.</returns>
        private static string GetEnglishMessage(string key)
        {
            return key switch
            {
                ErrorMessageKeys.InternalServerError => DefaultErrorMessages.InternalServerError,
                ErrorMessageKeys.ValidationError => DefaultErrorMessages.ValidationError,
                ErrorMessageKeys.NotFound => DefaultErrorMessages.NotFound,
                ErrorMessageKeys.AccessDenied => DefaultErrorMessages.AccessDenied,
                ErrorMessageKeys.InconsistentState => DefaultErrorMessages.InconsistentState,
                ErrorMessageKeys.BusinessRule => DefaultErrorMessages.BusinessRule,
                _ => key
            };
        }

        /// <summary>
        /// Gets the Portuguese (Brazil) version of an error message.
        /// </summary>
        /// <param name="key">The message key.</param>
        /// <returns>The Portuguese (Brazil) error message corresponding to the key, 
        /// or the English version if not found.</returns>
        private static string GetPortugueseBRMessage(string key)
        {
            return key switch
            {
                ErrorMessageKeys.InternalServerError => PortugueseBRErrorMessages.InternalServerError,
                ErrorMessageKeys.ValidationError => PortugueseBRErrorMessages.ValidationError,
                ErrorMessageKeys.NotFound => PortugueseBRErrorMessages.NotFound,
                ErrorMessageKeys.AccessDenied => PortugueseBRErrorMessages.AccessDenied,
                ErrorMessageKeys.InconsistentState => PortugueseBRErrorMessages.InconsistentState,
                ErrorMessageKeys.BusinessRule => PortugueseBRErrorMessages.BusinessRule,
                _ => GetEnglishMessage(key)
            };
        }
    }
}