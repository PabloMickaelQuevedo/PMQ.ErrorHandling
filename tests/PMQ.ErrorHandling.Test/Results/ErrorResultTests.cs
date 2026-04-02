namespace PMQ.ErrorHandling.Test.Results;

public class ErrorResultTests
{
    [Fact]
    public void Constructor_WithErrorDetails_ShouldSetStatusCodeFromErrorDetails()
    {
        // Arrange
        var errorDetails = new ErrorDetails { Status = 404 };

        // Act
        var result = new ErrorResult(errorDetails);

        // Assert
        result.StatusCode.ShouldBe(404);
        result.Value.ShouldBe(errorDetails);
    }

    [Fact]
    public void Constructor_WithErrorDetailsStatusNull_ShouldDefaultTo400()
    {
        // Arrange
        var errorDetails = new ErrorDetails { Status = null };

        // Act
        var result = new ErrorResult(errorDetails);

        // Assert
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Constructor_WithErrorDetailsStatus500_ShouldSetStatusTo500()
    {
        // Arrange
        var errorDetails = new ErrorDetails { Status = 500 };

        // Act
        var result = new ErrorResult(errorDetails);

        // Assert
        result.StatusCode.ShouldBe(500);
    }

    [Fact]
    public void From_ShouldCreateErrorResultWithStatusCode()
    {
        // Arrange
        var message = "Custom error message";
        var statusCode = 422;

        // Act
        var result = ErrorResult.From(message, statusCode);

        // Assert
        result.StatusCode.ShouldBe(422);
        result.Value.ShouldBeOfType<ErrorDetails>();
        ((ErrorDetails)result.Value!).Title.ShouldBe(message);
        ((ErrorDetails)result.Value!).Status.ShouldBe(statusCode);
    }

    [Fact]
    public void From_ShouldCreateValidErrorResult()
    {
        // Arrange
        var message = "Not Found";
        var statusCode = 404;

        // Act
        var result = ErrorResult.From(message, statusCode);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ErrorResult>();
    }

    [Fact]
    public void ErrorResult_ShouldInheritFromObjectResult()
    {
        // Arrange & Act
        var errorDetails = new ErrorDetails { Status = 400 };
        var result = new ErrorResult(errorDetails);

        // Assert
        result.ShouldBeAssignableTo<ObjectResult>();
    }
}
