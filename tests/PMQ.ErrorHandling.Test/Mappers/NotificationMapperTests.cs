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
        // Notification(key, message): the key identifies the field, the message describes the failure.
        var notification = new Notifications.Notification("Name", "Field is required");
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
            new("Field1", "Error 1"),
            new("Field2", "Error 2"),
            new("Field3", "Error 3")
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
        var notification = new Notifications.Notification("Username", "Invalid value");
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
        var notification = new Notifications.Notification("Email", errorMessage);
        var notifications = new List<Notifications.Notification> { notification };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Message.ShouldBe(errorMessage);
    }

    [Fact]
    public void ToValidationErrors_WithoutKey_ShouldLeaveFieldNull()
    {
        // Arrange
        var notification = new Notifications.Notification("Something went wrong");
        var notifications = new List<Notifications.Notification> { notification };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Message.ShouldBe("Something went wrong");
        result[0].Field.ShouldBeNull();
    }

    [Fact]
    public void ToValidationErrors_ShouldNotSetCodeProperty()
    {
        // Arrange
        var notification = new Notifications.Notification("FieldName", "Error message");
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
            new("Field1", "First error"),
            new("Field2", "Second error"),
            new("Field3", "Third error")
        };

        // Act
        var result = notifications.ToValidationErrors().ToList();

        // Assert
        result[0].Message.ShouldBe("First error");
        result[1].Message.ShouldBe("Second error");
        result[2].Message.ShouldBe("Third error");
    }
}
