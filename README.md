# PMQ.ErrorHandling

A comprehensive ASP.NET Core error handling library that provides standardized error responses, automatic exception filtering, and business rule notification processing with built-in localization support.

## Features

- 🛡️ **Automatic Exception Handling** - Catches unhandled exceptions and returns standardized error responses
- 📋 **Business Rule Validation** - Processes notifications from the PMQ.Notifications package
- 🌍 **Multi-Language Support** - Built-in localization for English and Portuguese-BR
- 📍 **Trace ID Tracking** - Automatic trace ID generation using Activity.Current or HttpContext
- 🎯 **Standardized Responses** - All errors follow the RFC 7231 ProblemDetails format
- ⚙️ **Highly Configurable** - Customize messages, culture, and exception detail inclusion
- ✅ **Validation Error Details** - Rich validation error information with field-level feedback

## Installation

```bash
dotnet add package PMQ.ErrorHandling
```

## Quick Start

### 1. Register the Error Handling Services

In your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add error handling with default configuration
builder.Services.AddErrorHandling();

// Or with custom options
builder.Services.AddErrorHandling(options =>
{
    options.Culture = new CultureInfo("pt-BR");
    options.IncludeExceptionDetails = app.Environment.IsDevelopment();
    options.IncludeTraceId = true;
    options.CustomMessages["custom_key"] = "Your custom message";
});

var app = builder.Build();
```

### 2. Use in Your Controllers

The error handling works automatically! No additional code needed in your controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        // Unhandled exceptions are automatically caught and formatted
        var product = _productService.GetById(id);
        return Ok(product);
    }

    [HttpPost]
    public IActionResult CreateProduct(CreateProductRequest request)
    {
        // Validation errors via notifications are automatically processed
        var result = _productService.Create(request);
        return Ok(result);
    }
}
```

## Configuration

### Error Handling Options

```csharp
builder.Services.AddErrorHandling(options =>
{
    // Set the culture for localized messages (default: en-US)
    options.Culture = new CultureInfo("pt-BR");
    
    // Include exception details in responses (only for development!)
    options.IncludeExceptionDetails = app.Environment.IsDevelopment();
    
    // Include trace ID in error responses
    options.IncludeTraceId = true;
    
    // Add custom error messages
    options.CustomMessages.Add("custom_validation_error", "This is a custom error");
    options.CustomMessages.Add("business_rule_violation", "Business rule was violated");
});
```

## Supported Notification Types

The `NotificationFilter` automatically maps notification types to HTTP status codes:

| Notification Type | HTTP Status | Message Key |
|---|---|---|
| **NotFound** | 404 | NotFound |
| **AccessDenied** | 403 | AccessDenied |
| **InconsistentState** | 409 | InconsistentState |
| **BusinessRule** | 422 | BusinessRule |
| **Validation** | 400 | ValidationError |
| **Unknown** | 422 | ValidationError |

## Response Format

All error responses follow the RFC 7231 ProblemDetails format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": null,
  "instance": "/api/products",
  "traceId": "0HN1GH7ANBTCN:00000001",
  "errors": [
    {
      "field": "email",
      "message": "Invalid email format"
    },
    {
      "field": "age",
      "message": "Age must be greater than 18"
    }
  ]
}
```

## Usage Examples

### Example 1: Validation with Notifications

```csharp
[HttpPost]
public IActionResult CreateUser(CreateUserRequest request)
{
    // Assuming _userService uses PMQ.Notifications
    var result = _userService.Create(request);
    
    // If validation fails, the NotificationFilter automatically returns 400 with validation errors
    // If business rules fail, it returns 422 with appropriate error message
    // If successful, continue with your logic
    
    return Ok(result);
}
```

### Example 2: Using the Localizer Directly

```csharp
[ApiController]
public class MyController : ControllerBase
{
    private readonly IErrorLocalizer _localizer;
    
    public MyController(IErrorLocalizer localizer)
    {
        _localizer = localizer;
    }
    
    [HttpGet]
    public IActionResult GetData()
    {
        try
        {
            // Your business logic
        }
        catch (Exception ex)
        {
            var message = _localizer.Get(ErrorMessageKeys.InternalServerError);
            return StatusCode(500, new ErrorDetails 
            { 
                Title = message,
                Status = 500,
                Detail = ex.Message
            });
        }
    }
}
```

### Example 3: Creating Custom Error Responses

```csharp
[HttpDelete("{id}")]
public IActionResult DeleteProduct(int id)
{
    var product = _repository.GetById(id);
    
    if (product == null)
    {
        var localizer = HttpContext.RequestServices.GetRequiredService<IErrorLocalizer>();
        return NotFound(new ErrorDetails
        {
            Title = localizer.Get(ErrorMessageKeys.NotFound),
            Status = 404,
            TraceId = TraceHelper.GetTraceId(HttpContext)
        });
    }
    
    _repository.Delete(product);
    return NoContent();
}
```

### Example 4: Adding Custom Messages

```csharp
// In Program.cs
builder.Services.AddErrorHandling(options =>
{
    options.CustomMessages.Add("user_already_exists", "A user with this email already exists");
    options.CustomMessages.Add("invalid_payment", "The payment method is invalid");
});

// In your service or controller
var localizer = serviceProvider.GetRequiredService<IErrorLocalizer>();
var message = localizer.Get("user_already_exists"); // Returns the custom message
```

## Available Error Message Keys

Default error message keys (defined in `ErrorMessageKeys` class):

- `NotFound` - "The requested resource was not found"
- `AccessDenied` - "Access to this resource is denied"
- `InconsistentState` - "The system is in an inconsistent state"
- `BusinessRule` - "A business rule was violated"
- `ValidationError` - "Validation failed"
- `InternalServerError` - "An internal server error occurred"

## Localization

### Supported Cultures

- **en-US** (English) - Default
- **pt-BR** (Portuguese-Brazil)

### Adding a New Culture

You can extend localization by implementing a custom message provider:

```csharp
// Define your messages
public static class SpanishErrorMessages
{
    public const string ValidationError = "Error de validación";
    public const string NotFound = "Recurso no encontrado";
    public const string InternalServerError = "Error interno del servidor";
    // ... more messages
}

// Register in Program.cs
builder.Services.AddErrorHandling(options =>
{
    options.Culture = new CultureInfo("es-ES");
    // Add Spanish messages
    options.CustomMessages["ValidationError"] = SpanishErrorMessages.ValidationError;
    options.CustomMessages["NotFound"] = SpanishErrorMessages.NotFound;
});
```

## Trace ID Handling

The library automatically generates and includes trace IDs in error responses:

```csharp
// The TraceHelper retrieves the current Activity ID or falls back to HttpContext.TraceIdentifier
var traceId = TraceHelper.GetTraceId(httpContext);

// This is automatically set in error responses when IncludeTraceId = true
```

This helps with:
- 🔍 **Error Tracking** - Correlate errors with log entries
- 🐛 **Debugging** - Quickly find related logs
- 📊 **Analytics** - Track error patterns over time

## Integration with PMQ.Notifications

This library is designed to work seamlessly with `PMQ.Notifications`:

```csharp
public class UserService
{
    private readonly INotificationContext _notificationContext;
    
    public UserService(INotificationContext notificationContext)
    {
        _notificationContext = notificationContext;
    }
    
    public void CreateUser(CreateUserRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _notificationContext.AddNotification("email", "Email is required", NotificationType.Validation);
        }
        
        if (UserExists(request.Email))
        {
            _notificationContext.AddNotification("email", "Email already registered", NotificationType.BusinessRule);
        }
        
        // The NotificationFilter will automatically return appropriate error responses
        if (_notificationContext.HasNotifications)
            return;
        
        // Create user...
    }
}
```

## Response Examples

### Validation Error (400)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "traceId": "0HN1GH7ANBTCN:00000001",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    },
    {
      "field": "password",
      "message": "Password must be at least 8 characters"
    }
  ]
}
```

### Not Found Error (404)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "traceId": "0HN1GH7ANBTCN:00000002"
}
```

### Business Rule Error (422)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.22",
  "title": "Business Rule Violation",
  "status": 422,
  "traceId": "0HN1GH7ANBTCN:00000003",
  "errors": [
    {
      "field": "email",
      "message": "Email already registered"
    }
  ]
}
```

### Internal Server Error (500)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An internal server error occurred",
  "status": 500,
  "detail": "Object reference not set to an instance of an object.",
  "traceId": "0HN1GH7ANBTCN:00000004"
}
```

## Best Practices

### ✅ Do

- Always include trace IDs for production debugging
- Use the `IErrorLocalizer` for user-facing messages
- Leverage the automatic filter for unhandled exceptions
- Use consistent error message keys across your application

### ❌ Don't

- Include sensitive information in error details
- Return raw exception messages to clients
- Skip proper error logging in application logs
- Ignore the structured error format

## Dependencies

- .NET 8.0
- Microsoft.AspNetCore.Mvc.Core 2.3.9
- PMQ.Notifications 1.0.7

## License

MIT License

## Support

For issues, questions, or contributions, please visit the project repository.

---

**Created with ❤️ for better error handling in ASP.NET Core applications**
