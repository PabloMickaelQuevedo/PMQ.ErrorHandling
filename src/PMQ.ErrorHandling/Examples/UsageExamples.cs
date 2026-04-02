namespace PMQ.ErrorHandling.Examples
{
    /*
     * EXAMPLE 1: Basic Setup in Program.cs
     * ====================================
     */
    
    // using PMQ.ErrorHandling.Extensions;
    // 
    // var builder = WebApplication.CreateBuilder(args);
    // 
    // builder.Services.AddControllers();
    // builder.Services.AddErrorHandling();  // Use default configuration
    // 
    // var app = builder.Build();
    // app.UseRouting();
    // app.MapControllers();
    // app.Run();

    /*
     * EXAMPLE 2: Development vs Production Configuration
     * ===================================================
     */
    
    // using PMQ.ErrorHandling.Extensions;
    // 
    // var builder = WebApplication.CreateBuilder(args);
    // var environment = builder.Environment;
    // 
    // builder.Services.AddControllers();
    // 
    // if (environment.IsDevelopment())
    // {
    //     // Development: Include details, use Portuguese
    //     builder.Services.AddErrorHandling(options =>
    //     {
    //         options.IncludeExceptionDetails = true;
    //         options.Culture = "pt-BR";
    //         options.IncludeTraceId = true;
    //     });
    // }
    // else
    // {
    //     // Production: Hide details, use English
    //     builder.Services.AddErrorHandling(options =>
    //     {
    //         options.IncludeExceptionDetails = false;
    //         options.Culture = "en-US";
    //         options.IncludeTraceId = true;
    //     });
    // }
    // 
    // var app = builder.Build();
    // app.UseRouting();
    // app.MapControllers();
    // app.Run();

    /*
     * EXAMPLE 3: Culture from Request Header
     * =======================================
     */
    
    // using PMQ.ErrorHandling.Extensions;
    // using System.Globalization;
    // 
    // var builder = WebApplication.CreateBuilder(args);
    // 
    // builder.Services.AddControllers();
    // builder.Services.AddErrorHandling(options =>
    // {
    //     // Culture will be dynamically set per request
    //     options.Culture = "en-US";  // Default culture
    // });
    // 
    // var app = builder.Build();
    // 
    // // Add middleware to read Accept-Language header
    // app.Use(async (context, next) =>
    // {
    //     var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
    //     if (!string.IsNullOrEmpty(acceptLanguage))
    //     {
    //         var culture = acceptLanguage.Split(',')[0].Trim();
    //         CultureInfo.CurrentCulture = new CultureInfo(culture);
    //     }
    //     await next();
    // });
    // 
    // app.UseRouting();
    // app.MapControllers();
    // app.Run();

    /*
     * EXAMPLE 4: Custom Error Messages
     * =================================
     */
    
    // using PMQ.ErrorHandling.Extensions;
    // 
    // var builder = WebApplication.CreateBuilder(args);
    // 
    // builder.Services.AddControllers();
    // builder.Services.AddErrorHandling(options =>
    // {
    //     options.Culture = "en-US";
    //     
    //     // Override specific error messages
    //     options.CustomMessages["InternalServerError"] = 
    //         "Oops! Something went wrong. Please contact support.";
    //     
    //     options.CustomMessages["ValidationError"] = 
    //         "Please check your input and try again.";
    // });
    // 
    // var app = builder.Build();
    // app.UseRouting();
    // app.MapControllers();
    // app.Run();

    /*
     * EXAMPLE 5: Using with PMQ.Notifications
     * ========================================
     */
    
    // using PMQ.ErrorHandling.Extensions;
    // using PMQ.Notifications;
    // 
    // [ApiController]
    // [Route("api/[controller]")]
    // public class ProductsController : ControllerBase
    // {
    //     private readonly INotificationContext _notificationContext;
    //     
    //     public ProductsController(INotificationContext notificationContext)
    //     {
    //         _notificationContext = notificationContext;
    //     }
    //     
    //     [HttpPost]
    //     public IActionResult Create(CreateProductRequest request)
    //     {
    //         // Validate business rules
    //         if (request.Price <= 0)
    //         {
    //             _notificationContext.AddNotification(
    //                 NotificationType.BusinessRule,
    //                 "Price must be greater than zero",
    //                 "Price"
    //             );
    //             return BadRequest();  // NotificationFilter will handle response
    //         }
    //         
    //         // ... rest of implementation
    //         
    //         return Ok(product);
    //     }
    //     
    //     [HttpGet("{id}")]
    //     public IActionResult GetById(int id)
    //     {
    //         var product = FindProduct(id);
    //         
    //         if (product == null)
    //         {
    //             _notificationContext.AddNotification(
    //                 NotificationType.NotFound,
    //                 $"Product with ID {id} not found"
    //             );
    //             return NotFound();  // NotificationFilter will handle response
    //         }
    //         
    //         return Ok(product);
    //     }
    // }

    /*
     * EXAMPLE 6: Exception Handling Example
     * =====================================
     */
    
    // using PMQ.ErrorHandling.Extensions;
    // 
    // [ApiController]
    // [Route("api/[controller]")]
    // public class OrdersController : ControllerBase
    // {
    //     [HttpPost]
    //     public IActionResult CreateOrder(CreateOrderRequest request)
    //     {
    //         try
    //         {
    //             // If an unhandled exception occurs here,
    //             // ExceptionFilter will automatically catch it and
    //             // return a standardized error response
    //             
    //             var order = new Order { ... };
    //             _database.SaveChanges();
    //             
    //             return CreatedAtAction(nameof(GetOrder), order);
    //         }
    //         catch (ArgumentException ex)
    //         {
    //             // For validation-specific exceptions, you can handle them
    //             // explicitly and use NotificationContext
    //             _notificationContext.AddNotification(
    //                 NotificationType.Validation,
    //                 ex.Message
    //             );
    //             return BadRequest();
    //         }
    //     }
    // }

    /*
     * EXAMPLE 7: Response Format Examples
     * ===================================
     */
    
    /*
     * 1. Validation Error Response (400 Bad Request):
     * 
     * {
     *   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
     *   "title": "One or more validation errors occurred.",
     *   "status": 400,
     *   "traceId": "0HMVJE4LG7N42:00000001",
     *   "errors": [
     *     {
     *       "message": "The Name field is required.",
     *       "field": "Name"
     *     },
     *     {
     *       "message": "The Email field is not a valid e-mail address.",
     *       "field": "Email"
     *     }
     *   ]
     * }
     * 
     * 2. Not Found Error Response (404 Not Found):
     * 
     * {
     *   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
     *   "title": "Resource not found.",
     *   "status": 404,
     *   "traceId": "0HMVJE4LG7N42:00000002",
     *   "errors": []
     * }
     * 
     * 3. Business Rule Error Response (422 Unprocessable Entity):
     * 
     * {
     *   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.22",
     *   "title": "A business rule validation failed.",
     *   "status": 422,
     *   "traceId": "0HMVJE4LG7N42:00000003",
     *   "errors": [
     *     {
     *       "message": "Cannot process order with total amount less than $10",
     *       "field": "Total"
     *     }
     *   ]
     * }
     * 
     * 4. Server Error Response (500 Internal Server Error):
     * 
     * With IncludeExceptionDetails = false:
     * {
     *   "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
     *   "title": "An unexpected error occurred.",
     *   "status": 500,
     *   "traceId": "0HMVJE4LG7N42:00000004"
     * }
     * 
     * With IncludeExceptionDetails = true:
     * {
     *   "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
     *   "title": "An unexpected error occurred.",
     *   "status": 500,
     *   "detail": "Object reference not set to an instance of an object.",
     *   "traceId": "0HMVJE4LG7N42:00000004"
     * }
     */

    /*
     * EXAMPLE 8: Localized Responses
     * ==============================
     */
    
    /*
     * Same error with different cultures:
     * 
     * Culture: en-US
     * {
     *   "title": "One or more validation errors occurred.",
     *   "status": 400
     * }
     * 
     * Culture: pt-BR
     * {
     *   "title": "Um ou mais erros de validação ocorreram.",
     *   "status": 400
     * }
     * 
     * With custom message:
     * {
     *   "title": "Por favor, verifique seus dados e tente novamente.",
     *   "status": 400
     * }
     */

    /*
     * EXAMPLE 9: Dependency Injection in Custom Services
     * ==================================================
     */
    
    // using PMQ.ErrorHandling.Interfaces;
    // 
    // public class ProductService
    // {
    //     private readonly IErrorLocalizer _localizer;
    //     
    //     public ProductService(IErrorLocalizer localizer)
    //     {
    //         _localizer = localizer;
    //     }
    //     
    //     public void ValidateProduct(Product product)
    //     {
    //         if (string.IsNullOrEmpty(product.Name))
    //         {
    //             var message = _localizer.Get("ValidationError");
    //             throw new ValidationException(message);
    //         }
    //     }
    // }

    /*
     * EXAMPLE 10: Configuration in appsettings.json
     * =============================================
     */
    
    /*
     * appsettings.json:
     * 
     * {
     *   "ErrorHandling": {
     *     "IncludeExceptionDetails": false,
     *     "IncludeTraceId": true,
     *     "Culture": "en-US",
     *     "CustomMessages": {
     *       "InternalServerError": "Contact support if the problem persists.",
     *       "ValidationError": "Please verify your input data."
     *     }
     *   }
     * }
     * 
     * Program.cs:
     * 
     * var builder = WebApplication.CreateBuilder(args);
     * 
     * var errorHandlingConfig = builder.Configuration.GetSection("ErrorHandling");
     * builder.Services.AddErrorHandling(options =>
     * {
     *     errorHandlingConfig.Bind(options);
     * });
     */
}
