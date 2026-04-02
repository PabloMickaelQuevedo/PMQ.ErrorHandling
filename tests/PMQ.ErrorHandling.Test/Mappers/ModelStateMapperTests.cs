namespace PMQ.ErrorHandling.Test.Mappers;

public class ModelStateMapperTests
{
    [Fact]
    public void ToValidationErrors_WithEmptyModelState_ShouldReturnEmptyList()
    {
        // Arrange
        var modelState = new ModelStateDictionary();

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToValidationErrors_WithValidField_ShouldReturnEmptyList()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Error occurred");
        modelState.SetModelValue("Name", new ValueProviderResult("value"), "");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(1);
    }

    [Fact]
    public void ToValidationErrors_WithSingleError_ShouldReturnSingleValidationError()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Email", "Invalid email format");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Message.ShouldBe("Invalid email format");
        result[0].Field.ShouldBe("Email");
    }

    [Fact]
    public void ToValidationErrors_WithMultipleErrorsOnSameField_ShouldReturnMultipleValidationErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Password", "Password is required");
        modelState.AddModelError("Password", "Password must be at least 8 characters");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Field.ShouldBe("Password");
        result[1].Field.ShouldBe("Password");
        result[0].Message.ShouldBe("Password is required");
        result[1].Message.ShouldBe("Password must be at least 8 characters");
    }

    [Fact]
    public void ToValidationErrors_WithMultipleFields_ShouldReturnMultipleValidationErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Name is required");
        modelState.AddModelError("Email", "Email is required");
        modelState.AddModelError("Age", "Age must be greater than 0");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(3);
        result.Any(e => e.Field == "Name").ShouldBeTrue();
        result.Any(e => e.Field == "Email").ShouldBeTrue();
        result.Any(e => e.Field == "Age").ShouldBeTrue();
    }

    [Fact]
    public void ToValidationErrors_ShouldMapFieldNameCorrectly()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Username", "Username already exists");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result[0].Field.ShouldBe("Username");
    }

    [Fact]
    public void ToValidationErrors_ShouldNotIncludeFieldsWithoutErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Email", "Invalid email");
        modelState.SetModelValue("Name", new ValueProviderResult("ValidName"), "");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Field.ShouldBe("Email");
    }

    [Fact]
    public void ToValidationErrors_ShouldNotSetCodeProperty()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field", "Error message");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result[0].Code.ShouldBeNull();
    }

    [Fact]
    public void ToValidationErrors_ShouldPreserveErrorMessageExactly()
    {
        // Arrange
        var errorMessage = "Field must match pattern: ^[A-Z][a-z]*$";
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Pattern", errorMessage);

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result[0].Message.ShouldBe(errorMessage);
    }

    [Fact]
    public void ToValidationErrors_WithCombinedErrorsAndValidFields_ShouldReturnOnlyErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("InvalidField", "This is invalid");
        modelState.SetModelValue("ValidField1", new ValueProviderResult("value1"), "");
        modelState.AddModelError("AnotherInvalidField", "This is also invalid");
        modelState.SetModelValue("ValidField2", new ValueProviderResult("value2"), "");

        // Act
        var result = modelState.ToValidationErrors().ToList();

        // Assert
        result.Count.ShouldBe(2);
        result.All(e => e.Field != "ValidField1" && e.Field != "ValidField2").ShouldBeTrue();
    }
}
