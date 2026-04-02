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
/// This filter is automatically registered via the AddErrorHandling extension method.
/// It handles all unhandled exceptions in action methods and converts them into
/// properly formatted <see cref="ErrorDetails"/> responses.
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
