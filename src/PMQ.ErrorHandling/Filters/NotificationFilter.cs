using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using PMQ.ErrorHandling.Constants;
using PMQ.ErrorHandling.Helpers;
using PMQ.ErrorHandling.Interfaces;
using PMQ.ErrorHandling.Mappers;
using PMQ.ErrorHandling.Models;
using PMQ.ErrorHandling.Results;
using PMQ.Notifications;

namespace PMQ.ErrorHandling.Filters
{
    /// <summary>
    /// Action filter that processes notifications from the PMQ.Notifications package 
    /// and converts them into standardized error responses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This filter is automatically registered via the AddErrorHandling extension method.
    /// It runs after action execution to check for any pending notifications
    /// and converts them into appropriate HTTP status code responses.
    /// </para>
    /// <para>
    /// Notification types are mapped to HTTP status codes as follows:
    /// <list type="table">
    /// <listheader><term>Notification Type</term><term>HTTP Status</term><term>Message Key</term></listheader>
    /// <item><term>NotFound</term><term>404</term><term>NotFound</term></item>
    /// <item><term>AccessDenied</term><term>403</term><term>AccessDenied</term></item>
    /// <item><term>InconsistentState</term><term>409</term><term>InconsistentState</term></item>
    /// <item><term>BusinessRule</term><term>422</term><term>BusinessRule</term></item>
    /// <item><term>Validation</term><term>400</term><term>ValidationError</term></item>
    /// <item><term>Unknown</term><term>422</term><term>ValidationError</term></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class NotificationFilter(
        INotificationContext context,
        IErrorLocalizer localizer) : IActionFilter
    {
        private readonly INotificationContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IErrorLocalizer _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        /// <summary>
        /// Processes notifications after action execution and creates error responses if needed.
        /// </summary>
        /// <param name="context">The action executed context containing the action result and context.</param>
        /// <remarks>
        /// <para>
        /// This method checks if the notification context contains any notifications.
        /// If notifications exist, it:
        /// <list type="number">
        /// <item><description>Converts notifications to validation errors</description></item>
        /// <item><description>Determines the appropriate HTTP status code based on the notification type</description></item>
        /// <item><description>Retrieves a localized error title based on the notification type</description></item>
        /// <item><description>Returns an <see cref="ErrorDetails"/> response with the appropriate status code</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!_context.HasNotifications) return;

            var errors = _context.Notifications.ToValidationErrors();
            var firstNotificationType = _context.Notifications.FirstOrDefault()?.Type;

            var status = firstNotificationType switch
            {
                NotificationType when firstNotificationType == NotificationType.NotFound => StatusCodes.Status404NotFound,
                NotificationType when firstNotificationType == NotificationType.AccessDenied => StatusCodes.Status403Forbidden,
                NotificationType when firstNotificationType == NotificationType.InconsistentState => StatusCodes.Status409Conflict,
                NotificationType when firstNotificationType == NotificationType.BusinessRule => StatusCodes.Status422UnprocessableEntity,
                NotificationType when firstNotificationType == NotificationType.Validation => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status422UnprocessableEntity
            };

            var titleKey = firstNotificationType switch
            {
                NotificationType when firstNotificationType == NotificationType.NotFound => ErrorMessageKeys.NotFound,
                NotificationType when firstNotificationType == NotificationType.AccessDenied => ErrorMessageKeys.AccessDenied,
                NotificationType when firstNotificationType == NotificationType.InconsistentState => ErrorMessageKeys.InconsistentState,
                NotificationType when firstNotificationType == NotificationType.BusinessRule => ErrorMessageKeys.BusinessRule,
                NotificationType when firstNotificationType == NotificationType.Validation => ErrorMessageKeys.ValidationError,
                _ => ErrorMessageKeys.ValidationError
            };

            var error = new ErrorDetails
            {
                Title = _localizer.Get(titleKey),
                Status = status,
                Errors = errors,
                TraceId = TraceHelper.GetTraceId(context.HttpContext)
            };

            context.Result = new ErrorResult(error);
        }

        /// <summary>
        /// Called before action execution. This implementation does nothing.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        public void OnActionExecuting(ActionExecutingContext context) { }
    }
}
