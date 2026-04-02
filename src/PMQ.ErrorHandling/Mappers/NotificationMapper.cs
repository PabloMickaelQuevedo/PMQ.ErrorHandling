using PMQ.ErrorHandling.Models;

namespace PMQ.ErrorHandling.Mappers;

public static class NotificationMapper
{
    public static IEnumerable<ValidationError> ToValidationErrors(this IEnumerable<Notifications.Notification> notifications)
    {
        return notifications.Select(n =>
            new ValidationError(n.Key, n.Message));
    }
}