using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PMQ.ErrorHandling.Constants;
using PMQ.ErrorHandling.Filters;
using PMQ.ErrorHandling.Interfaces;
using PMQ.ErrorHandling.Localization;
using PMQ.ErrorHandling.Mappers;
using PMQ.ErrorHandling.Models;
using PMQ.ErrorHandling.Options;

namespace PMQ.ErrorHandling.Extensions;

/// <summary>
/// Extension methods for configuring error handling services in an ASP.NET Core application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds error handling services to the dependency injection container with default configuration.
    /// </summary>
    /// <param name="services">The service collection to add error handling services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> to enable method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method registers:
    /// <list type="bullet">
    /// <item><description><see cref="IErrorLocalizer"/> - For localizing error messages</description></item>
    /// <item><description><see cref="ExceptionFilter"/> - For handling unhandled exceptions</description></item>
    /// <item><description><see cref="NotificationFilter"/> - For handling notifications from PMQ.Notifications</description></item>
    /// <item><description>Invalid model state response factory - For formatting validation errors</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The default configuration uses English language ("en-US") and does not expose exception details.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddErrorHandling();
    /// </code>
    /// </example>
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        return AddErrorHandling(services, _ => { });
    }

    /// <summary>
    /// Adds error handling services to the dependency injection container with custom configuration.
    /// </summary>
    /// <param name="services">The service collection to add error handling services to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="ErrorHandlingOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> to enable method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method registers all necessary error handling components and allows customization
    /// through the <paramref name="configureOptions"/> action.
    /// </para>
    /// <para>
    /// Configuration options include:
    /// <list type="bullet">
    /// <item><description>Culture selection (English or Portuguese Brazil)</description></item>
    /// <item><description>Exception details exposure</description></item>
    /// <item><description>Trace ID inclusion</description></item>
    /// <item><description>Custom error messages</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddErrorHandling(options =>
    /// {
    ///     options.Culture = "pt-BR";
    ///     options.IncludeExceptionDetails = false;
    ///     options.CustomMessages.Add("InternalServerError", "Custom message");
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddErrorHandling(
        this IServiceCollection services,
        Action<ErrorHandlingOptions> configureOptions)
    {
        // Register options
        services.Configure(configureOptions);

        // Register localizer
        services.AddScoped<IErrorLocalizer, DefaultErrorLocalizer>();

        // Register notification context if not already registered
        if (!services.Any(x => x.ServiceType == typeof(Notifications.INotificationContext)))
        {
            services.AddScoped<Notifications.INotificationContext>(sp => 
                new Notifications.NotificationContext());
        }

        // Register filters
        services.AddScoped<ExceptionFilter>();
        services.AddScoped<NotificationFilter>();

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<NotificationFilter>();
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.ToValidationErrors();

                var error = new ErrorDetails
                {
                    Title = ErrorMessageKeys.ValidationError,
                    Status = StatusCodes.Status400BadRequest,
                    Errors = errors
                };

                return new Results.ErrorResult(error);
            };
        });

        return services;
    }
}