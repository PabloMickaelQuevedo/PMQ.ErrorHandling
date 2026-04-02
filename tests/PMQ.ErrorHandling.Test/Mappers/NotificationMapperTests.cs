namespace PMQ.ErrorHandling.Test.Mappers;

public class NotificationMapperTests
{
    [Fact]
    public void ToValidationErrors_WithEmptyNotifications_ShouldReturnEmptyList()
    {
        // Arrange
        var notifications = new List<Notifications.Notification>();

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToValidationErrors_WithSingleNotification_ShouldReturnSingleValidationError()
    {
        // Arrange
        var notification = new Notifications.Notification("Field is required", "Name");
        var notifications = new List<Notifications.Notification> { notification };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Message.ShouldBe("Field is required");
        result[0].Field.ShouldBe("Name");
    }

    [Fact]
    public void ToValidationErrors_WithMultipleNotifications_ShouldReturnMultipleValidationErrors()
    {
        // Arrange
        var notifications = new List<Notifications.Notification>
        {
            new("Error 1", "Field1"),
            new("Error 2", "Field2"),
            new("Error 3", "Field3")
        };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(3);
        result[0].Message.ShouldBe("Error 1");
        result[1].Message.ShouldBe("Error 2");
        result[2].Message.ShouldBe("Error 3");
    }

    [Fact]
    public void ToValidationErrors_ShouldMapNotificationKeyToValidationErrorField()
    {
        // Arrange
        var notification = new Notifications.Notification("Invalid value", "Username");
        var notifications = new List<Notifications.Notification> { notification };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Field.ShouldBe("Username");
    }

    [Fact]
    public void ToValidationErrors_ShouldMapNotificationMessageToValidationErrorMessage()
    {
        // Arrange
        var errorMessage = "This field must be a valid email";
        var notification = new Notifications.Notification(errorMessage, "Email");
        var notifications = new List<Notifications.Notification> { notification };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Message.ShouldBe(errorMessage);
    }

    [Fact]
    public void ToValidationErrors_ShouldNotSetCodeProperty()
    {
        // Arrange
        var notification = new Notifications.Notification("Error message", "FieldName");
        var notifications = new List<Notifications.Notification> { notification };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Code.ShouldBeNull();
    }

    [Fact]
    public void ToValidationErrors_ShouldPreserveOrderOfNotifications()
    {
        // Arrange
        var notifications = new List<Notifications.Notification>
        {
            new("First error", "Field1"),
            new("Second error", "Field2"),
            new("Third error", "Field3")
        };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Message.ShouldBe("First error");
        result[1].Message.ShouldBe("Second error");
        result[2].Message.ShouldBe("Third error");
    }
}
