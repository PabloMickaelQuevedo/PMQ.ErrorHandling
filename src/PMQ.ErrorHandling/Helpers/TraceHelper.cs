using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace PMQ.ErrorHandling.Helpers
{
    /// <summary>
    /// Provides helper methods for retrieving trace identifiers from HTTP contexts.
    /// </summary>
    public static class TraceHelper
    {
        /// <summary>
        /// Gets the trace identifier for the current HTTP context.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>The trace identifier from the Activity, HTTP context, or a newly generated GUID.</returns>
        public static string GetTraceId(HttpContext context)
        {
            return Activity.Current?.Id 
                ?? context.TraceIdentifier 
                ?? Guid.NewGuid().ToString();
        }
    }
}
