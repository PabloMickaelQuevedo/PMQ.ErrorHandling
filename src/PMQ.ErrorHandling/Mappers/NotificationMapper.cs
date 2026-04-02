using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Mappers;

/// <summary>
/// Provides extension methods for mapping <see cref="Notifications.Notification"/> objects to validation errors.
/// </summary>
public static class NotificationMapper
{
    /// <summary>
    /// Converts a collection of notifications to validation errors.
    /// </summary>
    /// <param name="notifications">The notifications to convert.</param>
    /// <returns>A collection of <see cref="ValidationError"/> objects.</returns>
    public static IEnumerable<ValidationError> ToValidationErrors(this IEnumerable<Notifications.Notification> notifications)
    {
        return notifications.Select(n =>
            new ValidationError(n.Key, n.Message));
    }
}