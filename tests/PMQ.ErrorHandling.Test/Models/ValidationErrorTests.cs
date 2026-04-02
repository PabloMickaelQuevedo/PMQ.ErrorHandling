namespace PMQ.ErrorHandling.Test.Models;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_WithAllParameters_ShouldInitializeProperties()
    {
        // Arrange
        var message = "Field is required";
        var field = "Name";
        var code = "REQUIRED";

        // Act
        var error = new ValidationError(message, field, code);

        // Assert
        error.Message.ShouldBe(message);
        error.Field.ShouldBe(field);
        error.Code.ShouldBe(code);
    }

    [Fact]
    public void Constructor_WithOnlyMessage_ShouldSetMessageAndNullifyOptionalFields()
    {
        // Arrange
        var message = "An error occurred";

        // Act
        var error = new ValidationError(message);

        // Assert
        error.Message.ShouldBe(message);
        error.Field.ShouldBeNull();
        error.Code.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndField_ShouldSetMessageAndFieldAndNullifyCode()
    {
        // Arrange
        var message = "Invalid email format";
        var field = "Email";

        // Act
        var error = new ValidationError(message, field);

        // Assert
        error.Message.ShouldBe(message);
        error.Field.ShouldBe(field);
        error.Code.ShouldBeNull();
    }

    [Fact]
    public void ValidationError_ShouldAllowEmptyMessage()
    {
        // Arrange & Act
        var error = new ValidationError(string.Empty);

        // Assert
        error.Message.ShouldBeEmpty();
    }

    [Fact]
    public void ValidationError_ShouldAllowNullField()
    {
        // Arrange & Act
        var error = new ValidationError("Error", field: null, "CODE");

        // Assert
        error.Message.ShouldBe("Error");
        error.Field.ShouldBeNull();
        error.Code.ShouldBe("CODE");
    }
}
