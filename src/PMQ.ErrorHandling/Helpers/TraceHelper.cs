using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace PMQ.ErrorHandling.Helpers
{
    public static class TraceHelper
    {
        public static string GetTraceId(HttpContext context)
        {
            return Activity.Current?.Id 
                ?? context.TraceIdentifier 
                ?? Guid.NewGuid().ToString();
        }
    }
}
