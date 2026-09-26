using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace PMQ.ErrorHandling.Internal;

/// <summary>
/// Puts ASP.NET Core's exception handler middleware at the outermost position of the pipeline.
/// </summary>
/// <remarks>
/// <para>
/// Unexpected exceptions go through <c>UseExceptionHandler</c> instead of an MVC exception
/// filter: the middleware covers the whole pipeline — not only action methods — and it is what
/// emits the error log, the <c>aspnetcore.diagnostics.exceptions</c> metric and the diagnostic
/// events that tracing relies on. An exception filter that handles the exception suppresses all
/// three.
/// </para>
/// <para>
/// Registering it here keeps the library's promise that <c>AddErrorHandling()</c> is all an
/// application needs. An application that also calls <c>UseExceptionHandler()</c> is unaffected:
/// the inner one handles the exception and this one never sees it.
/// </para>
/// <para>
/// In Development, the developer exception page that <c>WebApplication</c> adds sits inside this
/// middleware, so it still handles exceptions first there.
/// </para>
/// </remarks>
internal sealed class ExceptionHandlerStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        app.UseExceptionHandler();
        next(app);
    };
}
