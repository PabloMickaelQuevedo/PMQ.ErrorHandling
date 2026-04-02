namespace PMQ.ErrorHandling.Localization
{
    /*
     * HOW TO ADD NEW LANGUAGE SUPPORT
     * ================================
     * 
     * Follow these steps to add localization for a new language:
     * 
     * STEP 1: Add Message Constants
     * ==============================
     * 
     * In Constants/ErrorMessages.cs, add a new static class for the language:
     * 
     *     /// <summary>
     *     /// Error messages in German (Deutsch).
     *     /// </summary>
     *     public static class GermanErrorMessages
     *     {
     *         public const string InternalServerError = "Ein unerwarteter Fehler ist aufgetreten.";
     *         public const string ValidationError = "Ein oder mehrere Validierungsfehler sind aufgetreten.";
     *         public const string NotFound = "Ressource nicht gefunden.";
     *         public const string AccessDenied = "Zugriff verweigert.";
     *         public const string InconsistentState = "Operation führte zu einem inkonsistenten Zustand.";
     *         public const string BusinessRule = "Eine Geschäftsregelvalidierung ist fehlgeschlagen.";
     *     }
     * 
     * 
     * STEP 2: Update DefaultErrorLocalizer
     * =====================================
     * 
     * In Localization/DefaultErrorLocalizer.cs:
     * 
     * A) Add a private method to get messages for the new language:
     * 
     *     private static string GetGermanMessage(string key)
     *     {
     *         return key switch
     *         {
     *             ErrorMessageKeys.InternalServerError => GermanErrorMessages.InternalServerError,
     *             ErrorMessageKeys.ValidationError => GermanErrorMessages.ValidationError,
     *             ErrorMessageKeys.NotFound => GermanErrorMessages.NotFound,
     *             ErrorMessageKeys.AccessDenied => GermanErrorMessages.AccessDenied,
     *             ErrorMessageKeys.InconsistentState => GermanErrorMessages.InconsistentState,
     *             ErrorMessageKeys.BusinessRule => GermanErrorMessages.BusinessRule,
     *             _ => GetEnglishMessage(key)  // Fallback to English
     *         };
     *     }
     * 
     * B) Update the Get() method to check for the new culture:
     * 
     *     public string Get(string key)
     *     {
     *         // Check custom messages first
     *         if (_options.CustomMessages.TryGetValue(key, out var customMessage))
     *         {
     *             return customMessage;
     *         }
     * 
     *         // Get message based on configured culture
     *         return _options.Culture switch
     *         {
     *             _ when _options.Culture?.StartsWith("pt") ?? false => GetPortugueseBRMessage(key),
     *             _ when _options.Culture?.StartsWith("de") ?? false => GetGermanMessage(key),
     *             _ => GetEnglishMessage(key)
     *         };
     *     }
     * 
     * 
     * STEP 3: Test the Implementation
     * ================================
     * 
     * Test the new language with the following code:
     * 
     *     services.AddErrorHandling(options =>
     *     {
     *         options.Culture = "de-DE";
     *     });
     * 
     *     // Errors should now be displayed in German
     * 
     * 
     * STEP 4: Document in Configuration Guide
     * ========================================
     * 
     * Update README_CONFIGURATION.md to list the new supported language:
     * 
     *     ### German
     *     Use `options.Culture = "de-DE"` or any culture starting with "de".
     * 
     * 
     * NOTES
     * =====
     * 
     * - Always include a fallback to English for unknown message keys
     * - Use culture prefixes (e.g., "pt" for Portuguese) to support multiple regions
     * - Keep error messages concise and user-friendly
     * - Test with real culture-specific formatting (dates, numbers, etc.) if applicable
     * - Consider pluralization and gender-specific variations in languages that need them
     * - Add XML documentation comments to all new methods
     * 
     * EXAMPLE: Adding Italian Support
     * ================================
     * 
     * 1. Add to ErrorMessages.cs:
     * 
     *     public static class ItalianErrorMessages
     *     {
     *         public const string InternalServerError = "Si è verificato un errore imprevisto.";
     *         public const string ValidationError = "Uno o più errori di convalida si sono verificati.";
     *         public const string NotFound = "Risorsa non trovata.";
     *         public const string AccessDenied = "Accesso negato.";
     *         public const string InconsistentState = "L'operazione ha determinato uno stato incoerente.";
     *         public const string BusinessRule = "Una convalida della regola di business non riuscita.";
     *     }
     * 
     * 2. Add method to DefaultErrorLocalizer.cs:
     * 
     *     private static string GetItalianMessage(string key)
     *     {
     *         return key switch
     *         {
     *             ErrorMessageKeys.InternalServerError => ItalianErrorMessages.InternalServerError,
     *             ErrorMessageKeys.ValidationError => ItalianErrorMessages.ValidationError,
     *             ErrorMessageKeys.NotFound => ItalianErrorMessages.NotFound,
     *             ErrorMessageKeys.AccessDenied => ItalianErrorMessages.AccessDenied,
     *             ErrorMessageKeys.InconsistentState => ItalianErrorMessages.InconsistentState,
     *             ErrorMessageKeys.BusinessRule => ItalianErrorMessages.BusinessRule,
     *             _ => GetEnglishMessage(key)
     *         };
     *     }
     * 
     * 3. Update Get() method:
     * 
     *     return _options.Culture switch
     *     {
     *         _ when _options.Culture?.StartsWith("pt") ?? false => GetPortugueseBRMessage(key),
     *         _ when _options.Culture?.StartsWith("it") ?? false => GetItalianMessage(key),
     *         _ => GetEnglishMessage(key)
     *     };
     * 
     * 4. Test and document in README_CONFIGURATION.md
     */
}
