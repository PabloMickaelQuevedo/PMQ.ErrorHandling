using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PMQ.ErrorHandling.Constants;
using PMQ.ErrorHandling.Filters;
using PMQ.ErrorHandling.Helpers;
using PMQ.ErrorHandling.Interfaces;
using PMQ.ErrorHandling.Internal;
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
    /// <item><description><see cref="NotificationFilter"/> - For handling notifications from PMQ.Notifications</description></item>
    /// <item><description>Invalid model state response factory - For formatting validation errors</description></item>
    /// <item><description>Problem details customization - One shape, localized title and trace id for every error response</description></item>
    /// <item><description>The exception handler middleware, through an <c>IStartupFilter</c> - For unexpected exceptions anywhere in the pipeline, with logs, metrics and tracing</description></item>
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

        // Register filters. ExceptionFilter stays resolvable for applications that add it
        // explicitly, but is no longer part of the pipeline: see ExceptionHandlerStartupFilter.
        services.AddScoped<ExceptionFilter>();
        services.AddScoped<NotificationFilter>();

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<NotificationFilter>();
        });

        // One writer and one shape for every error response. PostConfigure so that it wraps
        // whatever the application configured: ours runs first, the application's can still
        // override any member.
        services.AddProblemDetails();
        services.PostConfigure<ProblemDetailsOptions>(options =>
        {
            var applicationCustomization = options.CustomizeProblemDetails;
            options.CustomizeProblemDetails = context =>
            {
                ErrorContract.Customize(context);
                applicationCustomization?.Invoke(context);
            };
        });

        // Unexpected exceptions go to ASP.NET Core's exception handler middleware, which logs,
        // records metrics and emits the diagnostic events tracing depends on.
        services.TryAddEnumerable(ServiceDescriptor.Transient<IStartupFilter, ExceptionHandlerStartupFilter>());

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.ToValidationErrors();

                // O localizer e o trace são resolvidos do escopo da requisição, como nos
                // filtros. Antes esta era a única via que devolvia a chave crua no Title
                // ("ValidationError") e deixava o TraceId nulo, enquanto ExceptionFilter e
                // NotificationFilter devolviam o título traduzido e o trace preenchido.
                var localizer = context.HttpContext.RequestServices.GetRequiredService<IErrorLocalizer>();

                var error = new ErrorDetails
                {
                    Title = localizer.Get(ErrorMessageKeys.ValidationError),
                    Status = StatusCodes.Status400BadRequest,
                    Errors = errors,
                    TraceId = TraceHelper.GetTraceId(context.HttpContext)
                };

                return new Results.ErrorResult(error);
            };
        });

        return services;
    }
}