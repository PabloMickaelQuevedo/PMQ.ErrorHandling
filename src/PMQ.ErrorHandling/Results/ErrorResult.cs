using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PMQ.ErrorHandling.Internal;
using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Results
{
    /// <summary>
    /// Represents an action result that returns error details with an appropriate HTTP status code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The body is an <see cref="ErrorDetails"/>, which is a <see cref="ProblemDetails"/> with the
    /// contract's extension members. It is written through <see cref="IProblemDetailsService"/> —
    /// the same writer ASP.NET Core uses for exceptions and status code pages — so every error
    /// response gets the same shape, the <c>type</c> member and <c>application/problem+json</c>,
    /// regardless of a <see cref="ProducesAttribute"/> on the controller.
    /// </para>
    /// <para>
    /// If no problem details writer accepts the request (for instance, an <c>Accept</c> header
    /// that excludes JSON), it falls back to regular MVC content negotiation.
    /// </para>
    /// </remarks>
    public class ErrorResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResult"/> class with error details.
        /// </summary>
        /// <param name="errorDetails">The error details to include in the response.</param>
        public ErrorResult(ErrorDetails errorDetails)
            : base(errorDetails)
        {
            StatusCode = errorDetails.Status ?? 400;
        }

        /// <summary>
        /// Creates a new <see cref="ErrorResult"/> from a message and status code.
        /// </summary>
        /// <param name="message">The error message to include in the response.</param>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        /// <returns>A new instance of <see cref="ErrorResult"/> with the specified message and status code.</returns>
        public static ErrorResult From (string message, int statusCode)
        {
            return new ErrorResult(new ErrorDetails
            {
                Title = message,
                Status = statusCode
            });
        }

        /// <inheritdoc />
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var httpContext = context.HttpContext;

            if (Value is ErrorDetails details
                && httpContext.RequestServices.GetService<IProblemDetailsService>() is { } problemDetails)
            {
                httpContext.Response.StatusCode = StatusCode ?? details.Status ?? StatusCodes.Status400BadRequest;

                // Written in its wire form: the framework's writers serialize by the base type
                // and would drop the members declared on ErrorDetails. See ContractProblemDetails.
                httpContext.Items[ErrorContract.TitleIsFinalKey] = true;
                try
                {
                    var written = await problemDetails.TryWriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = ContractProblemDetails.From(details),
                    }).ConfigureAwait(false);

                    if (written)
                        return;
                }
                finally
                {
                    httpContext.Items.Remove(ErrorContract.TitleIsFinalKey);
                }
            }

            await base.ExecuteResultAsync(context).ConfigureAwait(false);
        }
    }
}
