# PMQ.ErrorHandling

Standardized, localized error responses for ASP.NET Core APIs. Every error — an unexpected
exception anywhere in the pipeline, a business notification from PMQ.Notifications, invalid model
state, an unmatched route — reaches the client in the same RFC 9457 shape, and every unexpected
exception stays visible to logs, metrics and tracing.

## Features

- 🎯 **One error contract** - `application/problem+json` with the same members on every error path
- 🛡️ **Unexpected exceptions** - Handled by ASP.NET Core's exception handler middleware, so they are logged, counted and traced
- 📋 **Business notifications** - Notifications from PMQ.Notifications become 400/403/404/409/422 responses, never exceptions
- 🌍 **Localization** - English and Portuguese (Brazil), with per-key overrides
- 🔒 **Safe by default** - Exception messages are never sent to clients unless you opt in
- 📍 **Trace ID** - Every error response carries the current trace id to correlate with logs

## Installation

```bash
dotnet add package PMQ.ErrorHandling
```

## Quick Start

### 1. Register the Error Handling Services

In your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add error handling with default configuration
builder.Services.AddErrorHandling();

// Or with custom options
builder.Services.AddErrorHandling(options =>
{
    options.Culture = "pt-BR";
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    options.CustomMessages["custom_key"] = "Your custom message";
});

var app = builder.Build();

// Recommended: gives the same contract to responses that have no body of their own,
// such as an unmatched route (404), 401, 405 and 429.
app.UseStatusCodePages();

app.MapControllers();
app.Run();
```

`AddErrorHandling()` is all the exception handling needs: it registers ASP.NET Core's exception
handler middleware at the start of the pipeline. Calling `app.UseExceptionHandler()` yourself is
harmless — the inner one handles the exception and the outer one never sees it.

### 2. Use in Your Controllers

No error handling code in controllers. Expected failures are notifications; unexpected ones are
exceptions you let propagate:

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // A missing product is a NotFound notification added by the handler: the
        // NotificationFilter turns it into a 404. A bug here throws, and becomes a logged 500.
        var product = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(product);
    }
}
```

## Configuration

### Error Handling Options

```csharp
builder.Services.AddErrorHandling(options =>
{
    // Culture for localized titles (default: en-US)
    options.Culture = "pt-BR";

    // Include the exception message in 500 responses (default: false — development only!)
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();

    // Include the trace id in error responses (default: true)
    options.IncludeTraceId = true;

    // Override any message by key
    options.CustomMessages["NotFound"] = "Nothing here.";
});
```

> **Since 1.1.5, `IncludeExceptionDetails` defaults to `false`.** Up to 1.1.4 it defaulted to
> `true`, so any application that did not set it returned exception messages — which routinely
> carry connection strings, SQL and file paths — in production responses. If you relied on
> seeing them while developing, set it explicitly as shown above.

### Customizing the problem details

Your own `ProblemDetailsOptions.CustomizeProblemDetails` keeps working and runs **after** this
library's, so it can add members or override anything:

```csharp
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["tenant"] = "acme");
```

## How It Works

| Where the error comes from | Who writes the response |
|---|---|
| Unexpected exception — action, filter, result serialization, middleware | ASP.NET Core's exception handler middleware |
| Notification from PMQ.Notifications | `NotificationFilter` → `ErrorResult` |
| Invalid model state (malformed body, binding failure) | `InvalidModelStateResponseFactory` → `ErrorResult` |
| Unmatched route, 401, 405, 429 without a body | `UseStatusCodePages()` |

All of them go through `IProblemDetailsService` and a single customization that sets the
localized title, decides whether the exception message is exposed and adds the trace id. That is
why they all read the same.

Unexpected exceptions are deliberately **not** handled by an MVC exception filter or an
`IExceptionHandler`. A filter that handles an exception suppresses its log entry, the
`aspnetcore.diagnostics.exceptions` metric and the diagnostic events tracing relies on. And an
`IExceptionHandler` exists to branch on the exception type, which there is no reason to do here:
business failures are notifications, so an exception is always a programming error and always a
500. On .NET 10, when an `IExceptionHandler` handles the exception the middleware also stops
writing the error log and the diagnostic events by default.

A client that disconnects mid-request is recorded by the middleware as aborted — not logged as
an error and not counted as a server failure.

## Supported Notification Types

The **first** notification's type decides the status of the response:

| Notification Type | HTTP Status | Title Key |
|---|---|---|
| **Validation** | 400 | ValidationError |
| **AccessDenied** | 403 | AccessDenied |
| **NotFound** | 404 | NotFound |
| **InconsistentState** | 409 | InconsistentState |
| **BusinessRule** | 422 | BusinessRule |
| *(no type)* | 422 | ValidationError |

## Response Format

Every error response is `application/problem+json` (RFC 9457). `errors` and `traceId` are
extension members; `detail` appears only on 500 responses with `IncludeExceptionDetails`
enabled.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-4d9388976dc457af47b2c28da7958b8f-8fd5f18d6c91939e-01",
  "errors": [
    { "message": "Email is required.", "field": "Email", "code": null }
  ]
}
```

In OpenAPI, document error responses with `ErrorDetails` — the type of the body — not
`ErrorResult`, which is the action result that writes it:

```csharp
[ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest, "application/problem+json")]
```

## Usage Examples

### Returning a specific error from a controller

Prefer a notification. When a controller has to answer directly, return an `ErrorResult` — it goes
through the same writer and customization as everything else:

```csharp
[HttpPost("{id}/cancel")]
public IActionResult Cancel(Guid id)
{
    if (!_orders.CanCancel(id))
        return ErrorResult.From("This order can no longer be cancelled.", StatusCodes.Status409Conflict);

    _orders.Cancel(id);
    return NoContent();
}
```

Returning `new ErrorDetails { ... }` through `NotFound(...)`, `StatusCode(...)` or `Ok(...)` skips
that writer and goes through MVC's formatters instead: no `type`, no trace id unless you set one,
none of your `CustomizeProblemDetails`, and a `[Produces]` on the controller can turn the media
type into plain `application/json`.

### Adding custom messages

```csharp
builder.Services.AddErrorHandling(options =>
{
    options.CustomMessages["user_already_exists"] = "A user with this email already exists.";
});

// In your code
var message = localizer.Get("user_already_exists");
```

## Available Error Message Keys

Defined in `ErrorMessageKeys`. Titles in English (`DefaultErrorMessages`) and Portuguese
(`PortugueseBRErrorMessages`):

| Key | Status | English | Português |
|---|---|---|---|
| `ValidationError` | 400 | One or more validation errors occurred. | Um ou mais erros de validação ocorreram. |
| `Unauthorized` | 401 | Authentication is required. | Autenticação necessária. |
| `AccessDenied` | 403 | Access denied. | Acesso negado. |
| `NotFound` | 404 | Resource not found. | Recurso não encontrado. |
| `MethodNotAllowed` | 405 | Method not allowed for this resource. | Método não permitido para este recurso. |
| `InconsistentState` | 409 | Operation resulted in an inconsistent state. | A operação resultou em um estado inconsistente. |
| `BusinessRule` | 422 | A business rule validation failed. | Uma validação de regra de negócio falhou. |
| `TooManyRequests` | 429 | Too many requests. Try again later. | Muitas requisições. Tente novamente mais tarde. |
| `InternalServerError` | 500 | An unexpected error occurred. | Um erro inesperado ocorreu. |

For a status without a key, the ASP.NET Core default title is kept.

## Localization

### Supported Cultures

- **en-US** (English) - Default
- **pt-BR** (Portuguese-Brazil) - Any culture starting with `pt`

### Adding a New Culture

Override the keys you need through `CustomMessages`:

```csharp
builder.Services.AddErrorHandling(options =>
{
    options.Culture = "es-ES";
    options.CustomMessages["ValidationError"] = "Error de validación.";
    options.CustomMessages["NotFound"] = "Recurso no encontrado.";
    options.CustomMessages["InternalServerError"] = "Error interno del servidor.";
});
```

## Integration with PMQ.Notifications

```csharp
public sealed class CreateUserCommandHandler(
    IUserRepository users,
    INotificationContext notificationContext) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await users.ExistsAsync(request.Email, cancellationToken))
        {
            // (key, message, type) — the key names the field, the message describes the failure.
            notificationContext.Add(nameof(request.Email), "Email already registered.", NotificationType.BusinessRule);
            return Guid.Empty;
        }

        // Create the user...
    }
}
```

The `NotificationFilter` turns the pending notifications into the response after the action runs.

## Upgrading to 1.2

1.2 routes unexpected exceptions through ASP.NET Core's exception handler middleware and gives
every error response the same writer. Your code does not change; the responses do:

- **500s are now logged, counted and traced.** Before, an exception from a controller was handled
  by an MVC exception filter and left no log entry, no metric and no trace event.
- **Every error is `application/problem+json`**, including notifications on controllers with
  `[Produces("application/json")]`, and every error has a `type`.
- **`errors` and `traceId` are omitted when empty** instead of being written as `null`.
- **Framework-written errors are localized** — an unmatched route, 401, 405, 429 and exceptions
  outside the MVC pipeline no longer come back with English titles.
- **`IncludeTraceId` is honored.** Up to 1.1.x it was not read anywhere.
- **`ExceptionFilter` is no longer registered.** The type remains for source compatibility.

## Dependencies

- .NET 8.0 or .NET 10.0 (ASP.NET Core shared framework)
- PMQ.Notifications

## License

MIT License

## Support

For issues, questions, or contributions, please visit the project repository.
