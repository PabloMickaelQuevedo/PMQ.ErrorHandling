using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PMQ.ErrorHandling.Constants;
using PMQ.ErrorHandling.Helpers;
using PMQ.ErrorHandling.Interfaces;
using PMQ.ErrorHandling.Options;

namespace PMQ.ErrorHandling.Internal;

/// <summary>
/// The single place that shapes every error response, whoever produced it.
/// </summary>
/// <remarks>
/// <para>
/// Plugged into <c>ProblemDetailsOptions.CustomizeProblemDetails</c>, so it runs for responses
/// written by <see cref="Results.ErrorResult"/> (notifications, invalid model state) and for
/// the ones ASP.NET Core writes itself: <c>UseExceptionHandler</c>, <c>UseStatusCodePages</c>
/// (unmatched route, empty 404, 401, 405, 429).
/// </para>
/// <para>
/// The framework has already applied its defaults when this runs — <c>type</c> from the status
/// code, an English title — so this only overrides what the contract owns: the localized title,
/// whether the exception message is exposed, and the trace id.
/// </para>
/// </remarks>
internal static class ErrorContract
{
    private const string TraceIdKey = ContractProblemDetails.TraceIdKey;

    private static readonly Dictionary<int, string> TitleKeys = new()
    {
        [StatusCodes.Status400BadRequest] = ErrorMessageKeys.ValidationError,
        [StatusCodes.Status401Unauthorized] = ErrorMessageKeys.Unauthorized,
        [StatusCodes.Status403Forbidden] = ErrorMessageKeys.AccessDenied,
        [StatusCodes.Status404NotFound] = ErrorMessageKeys.NotFound,
        [StatusCodes.Status405MethodNotAllowed] = ErrorMessageKeys.MethodNotAllowed,
        [StatusCodes.Status409Conflict] = ErrorMessageKeys.InconsistentState,
        [StatusCodes.Status422UnprocessableEntity] = ErrorMessageKeys.BusinessRule,
        [StatusCodes.Status429TooManyRequests] = ErrorMessageKeys.TooManyRequests,
        [StatusCodes.Status500InternalServerError] = ErrorMessageKeys.InternalServerError,
    };

    public static void Customize(ProblemDetailsContext context)
    {
        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;
        var options = services.GetRequiredService<IOptions<ErrorHandlingOptions>>().Value;
        var problem = context.ProblemDetails;

        // An ErrorResult arrives with a title chosen by whoever built it — a notification type,
        // invalid model state, ErrorResult.From. Only the framework's generic titles are replaced.
        if (problem is not ContractProblemDetails)
            LocalizeTitle(problem, httpContext, services);

        // Unexpected exceptions are programming errors: their message is never part of the
        // contract unless the application explicitly opted in, typically for Development.
        //
        // context.Exception is not enough. For controller endpoints the writer is MVC's, and it
        // calls this customization with a new context that does not carry the exception. The
        // exception handler middleware publishes it on the request, so it is read from there.
        var exception = context.Exception ?? httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is not null)
            problem.Detail = options.IncludeExceptionDetails ? exception.Message : null;

        ApplyTraceId(problem, httpContext, options);
    }

    private static void LocalizeTitle(Microsoft.AspNetCore.Mvc.ProblemDetails problem, HttpContext httpContext, IServiceProvider services)
    {
        var status = problem.Status ?? httpContext.Response.StatusCode;
        if (!TitleKeys.TryGetValue(status, out var key))
            return;

        // A custom IErrorLocalizer that does not know a key returns the key itself. Keeping the
        // framework's title in that case beats sending "TooManyRequests" to the client.
        var title = services.GetRequiredService<IErrorLocalizer>().Get(key);
        if (!string.Equals(title, key, StringComparison.Ordinal))
            problem.Title = title;
    }

    private static void ApplyTraceId(Microsoft.AspNetCore.Mvc.ProblemDetails problem, HttpContext httpContext, ErrorHandlingOptions options)
    {
        // One source for the trace id on every path and every target framework: on net8.0 the
        // framework omits it from some responses, and IncludeTraceId must be able to turn it off.
        if (options.IncludeTraceId)
            problem.Extensions[TraceIdKey] = TraceHelper.GetTraceId(httpContext);
        else
            problem.Extensions.Remove(TraceIdKey);
    }
}
