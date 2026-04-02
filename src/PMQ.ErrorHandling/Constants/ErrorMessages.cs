namespace PMQ.ErrorHandling.Constants
{
    /// <summary>
    /// Contains error message keys used for localization in the error handling system.
    /// </summary>
    /// <remarks>
    /// These keys are used to identify specific error types and retrieve localized
    /// messages from the error localizer. Always use these constants instead of hardcoded strings
    /// to ensure consistency and enable proper localization.
    /// </remarks>
    public static class ErrorMessageKeys
    {
        /// <summary>
        /// Key for internal server error message.
        /// Used when an unexpected exception occurs during request processing.
        /// </summary>
        public const string InternalServerError = "InternalServerError";

        /// <summary>
        /// Key for validation error message.
        /// Used when model validation fails.
        /// </summary>
        public const string ValidationError = "ValidationError";

        /// <summary>
        /// Key for not found error message.
        /// Used when a requested resource is not found (404).
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// Key for access denied error message.
        /// Used when a user lacks permissions to access a resource (403).
        /// </summary>
        public const string AccessDenied = "AccessDenied";

        /// <summary>
        /// Key for inconsistent state error message.
        /// Used when an operation results in data inconsistency (409).
        /// </summary>
        public const string InconsistentState = "InconsistentState";

        /// <summary>
        /// Key for business rule error message.
        /// Used when a business rule validation fails (422).
        /// </summary>
        public const string BusinessRule = "BusinessRule";
    }

    /// <summary>
    /// Default error messages in English.
    /// </summary>
    /// <remarks>
    /// These are the fallback messages used when no localization is available
    /// or when the requested culture is not supported.
    /// </remarks>
    public static class DefaultErrorMessages
    {
        /// <summary>
        /// Default message for internal server errors.
        /// </summary>
        public const string InternalServerError = "An unexpected error occurred.";

        /// <summary>
        /// Default message for validation errors.
        /// </summary>
        public const string ValidationError = "One or more validation errors occurred.";

        /// <summary>
        /// Default message for not found errors.
        /// </summary>
        public const string NotFound = "Resource not found.";

        /// <summary>
        /// Default message for access denied errors.
        /// </summary>
        public const string AccessDenied = "Access denied.";

        /// <summary>
        /// Default message for inconsistent state errors.
        /// </summary>
        public const string InconsistentState = "Operation resulted in an inconsistent state.";

        /// <summary>
        /// Default message for business rule errors.
        /// </summary>
        public const string BusinessRule = "A business rule validation failed.";
    }

    /// <summary>
    /// Error messages localized to Portuguese (Brazil).
    /// </summary>
    /// <remarks>
    /// These messages are used when the culture is set to "pt-BR" or any culture
    /// starting with "pt".
    /// </remarks>
    public static class PortugueseBRErrorMessages
    {
        /// <summary>
        /// Portuguese (Brazil) message for internal server errors.
        /// </summary>
        public const string InternalServerError = "Um erro inesperado ocorreu.";

        /// <summary>
        /// Portuguese (Brazil) message for validation errors.
        /// </summary>
        public const string ValidationError = "Um ou mais erros de validação ocorreram.";

        /// <summary>
        /// Portuguese (Brazil) message for not found errors.
        /// </summary>
        public const string NotFound = "Recurso não encontrado.";

        /// <summary>
        /// Portuguese (Brazil) message for access denied errors.
        /// </summary>
        public const string AccessDenied = "Acesso negado.";

        /// <summary>
        /// Portuguese (Brazil) message for inconsistent state errors.
        /// </summary>
        public const string InconsistentState = "A operação resultou em um estado inconsistente.";

        /// <summary>
        /// Portuguese (Brazil) message for business rule errors.
        /// </summary>
        public const string BusinessRule = "Uma validação de regra de negócio falhou.";
    }
}
