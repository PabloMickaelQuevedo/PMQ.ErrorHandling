using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using PMQ.ErrorHandling.Constants;
using PMQ.ErrorHandling.Helpers;
using PMQ.ErrorHandling.Interfaces;
using PMQ.ErrorHandling.Models;
using PMQ.ErrorHandling.Options;

namespace PMQ.ErrorHandling.Filters;

/// <summary>
/// Exception filter that catches unhandled exceptions and returns standardized error responses.
/// </summary>
/// <remarks>
/// <para>
/// <b>Since 1.2.0 this filter is no longer registered by <c>AddErrorHandling</c>.</b> Unexpected
/// exceptions go to ASP.NET Core's exception handler middleware instead, which covers the whole
/// pipeline rather than only action methods, and which logs them, records the
/// <c>aspnetcore.diagnostics.exceptions</c> metric and emits the diagnostic events tracing relies
/// on. A filter that handles the exception suppresses all three: with this filter in place, a 500
/// from a controller left no trace in logs, metrics or traces. The response body is produced by
/// the same error contract as every other error.
/// </para>
/// <para>
/// The type is kept so that applications referencing it keep compiling. Adding it back to the MVC
/// filters brings back the silent behavior described above.
/// </para>
/// <para>
/// Exception details are only included in the response when <see cref="ErrorHandlingOptions.IncludeExceptionDetails"/>
/// is set to <c>true</c>, which should only be done in development environments.
/// </para>
/// </remarks>
public class ExceptionFilter(
    IErrorLocalizer localizer,
    IOptions<ErrorHandlingOptions> options) : IExceptionFilter
{
    private readonly IErrorLocalizer _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    private readonly ErrorHandlingOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Handles the exception by creating a standardized error response.
    /// </summary>
    /// <param name="context">The exception context containing information about the exception.</param>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item><description>Retrieves a localized error message using the configured culture</description></item>
    /// <item><description>Creates an <see cref="ErrorDetails"/> object with the error information</description></item>
    /// <item><description>Conditionally includes exception details based on <see cref="ErrorHandlingOptions.IncludeExceptionDetails"/></description></item>
    /// <item><description>Returns an HTTP 500 status code response</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public void OnException(ExceptionContext context)
    {
        var message = _localizer.Get(ErrorMessageKeys.InternalServerError);

        var error = new ErrorDetails
        {
            Title = message,
            Status = 500,
            Detail = _options.IncludeExceptionDetails ? context.Exception.Message : null,
            TraceId = TraceHelper.GetTraceId(context.HttpContext)
        };

        context.Result = new Results.ErrorResult(error);
    }
}
