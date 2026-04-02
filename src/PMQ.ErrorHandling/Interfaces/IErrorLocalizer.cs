namespace PMQ.ErrorHandling.Interfaces
{
    /// <summary>
    /// Defines a contract for localizing error messages.
    /// </summary>
    public interface IErrorLocalizer
    {
        /// <summary>
        /// Gets a localized error message for the specified key.
        /// </summary>
        /// <param name="key">The error message key to retrieve.</param>
        /// <returns>The localized error message corresponding to the provided key.</returns>
        string Get(string key);
    }
}
