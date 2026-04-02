namespace PMQ.ErrorHandling.Test.Models;

public class ErrorDetailsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var errorDetails = new ErrorDetails();

        // Assert
        errorDetails.Errors.ShouldBeNull();
        errorDetails.TraceId.ShouldBeNull();
    }

    [Fact]
    public void Errors_ShouldBeSettable()
    {
        // Arrange
        var errorDetails = new ErrorDetails();
        var errors = new List<ValidationError>
        {
            new("Error 1", "Field1"),
            new("Error 2", "Field2")
        };

        // Act
        errorDetails.Errors = errors;

        // Assert
        errorDetails.Errors.ShouldNotBeNull();
        errorDetails.Errors.Count().ShouldBe(2);
    }

    [Fact]
    public void TraceId_ShouldBeSettable()
    {
        // Arrange
        var errorDetails = new ErrorDetails();
        var customTraceId = "custom-trace-id-123";

        // Act
        errorDetails.TraceId = customTraceId;

        // Assert
        errorDetails.TraceId.ShouldBe(customTraceId);
    }

    [Fact]
    public void Status_ShouldBeSettable()
    {
        // Arrange
        var errorDetails = new ErrorDetails();

        // Act
        errorDetails.Status = 400;

        // Assert
        errorDetails.Status.ShouldBe(400);
    }

    [Fact]
    public void Title_ShouldBeSettable()
    {
        // Arrange
        var errorDetails = new ErrorDetails();

        // Act
        errorDetails.Title = "Validation Error";

        // Assert
        errorDetails.Title.ShouldBe("Validation Error");
    }

    [Fact]
    public void Detail_ShouldBeSettable()
    {
        // Arrange
        var errorDetails = new ErrorDetails();

        // Act
        errorDetails.Detail = "Additional error details";

        // Assert
        errorDetails.Detail.ShouldBe("Additional error details");
    }

    [Fact]
    public void ErrorDetails_ShouldInheritFromProblemDetails()
    {
        // Arrange & Act
        var errorDetails = new ErrorDetails();

        // Assert
        errorDetails.ShouldBeAssignableTo<Microsoft.AspNetCore.Mvc.ProblemDetails>();
    }
}
