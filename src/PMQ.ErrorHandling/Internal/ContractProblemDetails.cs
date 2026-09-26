using Microsoft.AspNetCore.Mvc;
using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Internal;

/// <summary>
/// The wire form of an <see cref="ErrorDetails"/>: the same members, with the contract's extension
/// members carried in <see cref="ProblemDetails.Extensions"/>.
/// </summary>
/// <remarks>
/// <para>
/// ASP.NET Core's problem details writers serialize by the declared type,
/// <see cref="ProblemDetails"/>. Properties declared on a subclass are dropped — an
/// <see cref="ErrorDetails"/> written as is loses <c>errors</c> and <c>traceId</c>. The extension
/// data dictionary is the one channel every writer keeps, so the members travel there.
/// </para>
/// <para>
/// <see cref="ErrorDetails"/> stays the public, typed model: it is what the API documents and what
/// clients deserialize into. This type only exists between <see cref="Results.ErrorResult"/> and
/// the writer, and it also tells <see cref="ErrorContract"/> that the title was already chosen.
/// </para>
/// </remarks>
internal sealed class ContractProblemDetails : ProblemDetails
{
    internal const string ErrorsKey = "errors";
    internal const string TraceIdKey = "traceId";

    public static ContractProblemDetails From(ErrorDetails details)
    {
        var wire = new ContractProblemDetails
        {
            Type = details.Type,
            Title = details.Title,
            Status = details.Status,
            Detail = details.Detail,
            Instance = details.Instance,
        };

        foreach (var extension in details.Extensions)
            wire.Extensions[extension.Key] = extension.Value;

        // Materialized: Errors is often a deferred LINQ projection over the notification context.
        if (details.Errors is not null)
            wire.Extensions[ErrorsKey] = details.Errors.ToArray();

        if (details.TraceId is not null)
            wire.Extensions[TraceIdKey] = details.TraceId;

        return wire;
    }
}
