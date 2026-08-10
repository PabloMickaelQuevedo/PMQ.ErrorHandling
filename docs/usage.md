# Usage

Practical recipes for wiring PMQ.ErrorHandling into an ASP.NET Core application.

> Previously these examples lived in `src/PMQ.ErrorHandling/Examples/UsageExamples.cs` as
> commented-out code. Because the compiler never checked them, they drifted from the real API.
> Two of them documented a method that does not exist. They now live here, corrected.

## Basic setup

```csharp
using PMQ.ErrorHandling.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddErrorHandling();      // English, exception details on

var app = builder.Build();
app.MapControllers();
app.Run();
```

`AddErrorHandling` registers the exception filter, the notification filter, the error localizer
and the invalid-model-state response factory.

## Options

| Option | Default | Purpose |
|---|---|---|
| `IncludeExceptionDetails` | `true` | Adds the exception message to `detail` on 500 responses |
| `IncludeTraceId` | `true` | Adds `traceId` to the response |
| `Culture` | `"en-US"` | Any tag starting with `pt` selects Portuguese; anything else falls back to English |
| `CustomMessages` | empty | Overrides a message by key, taking precedence over the built-in ones |

### Development versus production

Exception details are useful while developing and are an information leak in production:

```csharp
builder.Services.AddErrorHandling(options =>
{
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();
    options.Culture = "pt-BR";
    options.IncludeTraceId = true;
});
```

### Custom messages

```csharp
builder.Services.AddErrorHandling(options =>
{
    options.CustomMessages["InternalServerError"] = "Something went wrong. Please contact support.";
    options.CustomMessages["ValidationError"] = "Please check your input and try again.";
});
```

Keys come from `ErrorMessageKeys`. A custom message wins over the localized default, so setting
one opts that key out of localization entirely.

### Binding from appsettings.json

```json
{
  "ErrorHandling": {
    "IncludeExceptionDetails": false,
    "IncludeTraceId": true,
    "Culture": "en-US",
    "CustomMessages": {
      "InternalServerError": "Contact support if the problem persists."
    }
  }
}
```

```csharp
builder.Services.AddErrorHandling(options =>
    builder.Configuration.GetSection("ErrorHandling").Bind(options));
```

## Business rules with PMQ.Notifications

Register the notification context and add notifications instead of throwing. The
`NotificationFilter` turns them into the matching status code after the action runs:

```csharp
builder.Services.AddScoped<INotificationContext, NotificationContext>();
```

```csharp
[ApiController]
[Route("products")]
public sealed class ProductsController(INotificationContext notifications) : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateProductRequest request)
    {
        if (request.Price <= 0)
        {
            // Add(key, message, type) — the key names the field, the message describes the failure.
            notifications.Add(nameof(request.Price), "Price must be greater than zero.", NotificationType.BusinessRule);
            return Ok();   // NotificationFilter replaces the result with 422
        }

        return Ok(/* ... */);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var product = FindProduct(id);

        if (product is null)
        {
            notifications.Add(nameof(id), $"Product {id} not found.", NotificationType.NotFound);
            return Ok();   // NotificationFilter replaces the result with 404
        }

        return Ok(product);
    }
}
```

The action's own return value does not matter once a notification exists — the filter overwrites
the result. Returning `Ok()` keeps the intent readable: the controller did its job, and the
notification decides the outcome.

### Status code mapping

The **first** notification's type decides the status code for the whole response:

| `NotificationType` | Status |
|---|---|
| `Validation` | 400 |
| `AccessDenied` | 403 |
| `NotFound` | 404 |
| `InconsistentState` | 409 |
| `BusinessRule` | 422 |
| none, or a `NotificationType.Custom(...)` | 422 |

## Unhandled exceptions

`ExceptionFilter` catches anything that escapes an action and returns a standardized 500. No
try/catch needed in the controller — reserve those for exceptions you can actually recover from.

## Response shape

`ErrorDetails` derives from `ProblemDetails`, so `title`, `status` and `detail` come from there;
`errors` and `traceId` are added by this package.

**Validation (400)**

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": [
    { "message": "The Name field is required.", "field": "Name" },
    { "message": "The Email field is not a valid e-mail address.", "field": "Email" }
  ],
  "traceId": "00-a28254f6ea4d37ae8ae1a50b23faa0a0-2dad26d1eb456032-01"
}
```

**Business rule (422)**

```json
{
  "title": "A business rule validation failed.",
  "status": 422,
  "errors": [
    { "message": "Price must be greater than zero.", "field": "Price" }
  ],
  "traceId": "00-c007e7cf87ae7ebf7f527107f9201bc6-8d53f44d510fd9d0-01"
}
```

**Server error (500)**

With `IncludeExceptionDetails = false`:

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "traceId": "00-079d247677d55f7d8427fd421daec5f0-a773dd27f85d9bdb-01"
}
```

With `IncludeExceptionDetails = true`, a `detail` field carries the exception message.

> `type` is inherited from `ProblemDetails` but this package does not populate it, so it is
> absent from the payload. Set it yourself if you publish a URI catalogue of error types.

## Localization

The same failure, under different cultures:

```jsonc
// Culture = "en-US"
{ "title": "One or more validation errors occurred.", "status": 400 }

// Culture = "pt-BR"
{ "title": "Um ou mais erros de validação ocorreram.", "status": 400 }
```

To add a language, see [extending-localization.md](extending-localization.md).

## Using the localizer directly

`IErrorLocalizer` is registered in the container and can be injected anywhere:

```csharp
public sealed class ProductService(IErrorLocalizer localizer)
{
    public void Validate(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ValidationException(localizer.Get(ErrorMessageKeys.ValidationError));
    }
}
```
