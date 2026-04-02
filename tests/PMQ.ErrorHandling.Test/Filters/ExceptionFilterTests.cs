using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace PMQ.ErrorHandling.Test.Filters
{
    public class ExceptionFilterTests
    {
        private readonly AutoMocker _mocker = new();

        private ExceptionFilter CreateFilter(
            IErrorLocalizer? localizer = null,
            ErrorHandlingOptions? options = null)
        {
            if (localizer != null)
                _mocker.Use(localizer);
            else
                _mocker.Use(Mock.Of<IErrorLocalizer>(m =>
                    m.Get(It.IsAny<string>()) == "Test Error Message"));

            if (options != null)
                _mocker.Use(Microsoft.Extensions.Options.Options.Create(options));
            else
                _mocker.Use(Microsoft.Extensions.Options.Options.Create(new ErrorHandlingOptions()));

            return _mocker.CreateInstance<ExceptionFilter>();
        }

        [Fact]
        public void OnException_ShouldReturnErrorResult()
        {
            // Arrange
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.InternalServerError) == DefaultErrorMessages.InternalServerError);

            var filter = CreateFilter(mockLocalizer);
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var exceptionContext = new ExceptionContext(
                new ActionContext(httpContext, new RouteData(), actionDescriptor),
                [])
            {
                Exception = new InvalidOperationException("Test exception")
            };

            // Act
            filter.OnException(exceptionContext);

            // Assert
            exceptionContext.Result.ShouldNotBeNull();
            exceptionContext.Result.ShouldBeOfType<ErrorResult>();
        }

        [Fact]
        public void OnException_ShouldSetStatusCodeTo500()
        {
            // Arrange
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(It.IsAny<string>()) == "Error Message");

            var filter = CreateFilter(mockLocalizer);
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var exceptionContext = new ExceptionContext(
                new ActionContext(httpContext, new RouteData(), actionDescriptor),
                [])
            {
                Exception = new Exception("Test exception")
            };

            // Act
            filter.OnException(exceptionContext);

            // Assert
            var errorResult = exceptionContext.Result as ErrorResult;
            errorResult.ShouldNotBeNull();
            errorResult.StatusCode.ShouldBe(500);
        }

        [Fact]
        public void OnException_WithIncludeExceptionDetailsTrue_ShouldIncludeExceptionMessage()
        {
            // Arrange
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(It.IsAny<string>()) == "Error Message");

            var options = new ErrorHandlingOptions { IncludeExceptionDetails = true };
            var filter = CreateFilter(mockLocalizer, options);
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();
            var exceptionMessage = "This is the exception message";

            var exceptionContext = new ExceptionContext(
                new ActionContext(httpContext, new RouteData(), actionDescriptor),
                [])
            {
                Exception = new Exception(exceptionMessage)
            };

            // Act
            filter.OnException(exceptionContext);

            // Assert
            var errorResult = exceptionContext.Result as ErrorResult;
            var errorDetails = errorResult?.Value as ErrorDetails;
            errorDetails?.Detail.ShouldBe(exceptionMessage);
        }

        [Fact]
        public void OnException_WithIncludeExceptionDetailsFalse_ShouldNotIncludeExceptionMessage()
        {
            // Arrange
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(It.IsAny<string>()) == "Error Message");

            var options = new ErrorHandlingOptions { IncludeExceptionDetails = false };
            var filter = CreateFilter(mockLocalizer, options);
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var exceptionContext = new ExceptionContext(
                new ActionContext(httpContext, new RouteData(), actionDescriptor),
                [])
            {
                Exception = new Exception("This should not be visible")
            };

            // Act
            filter.OnException(exceptionContext);

            // Assert
            var errorResult = exceptionContext.Result as ErrorResult;
            var errorDetails = errorResult?.Value as ErrorDetails;
            errorDetails?.Detail.ShouldBeNull();
        }

        [Fact]
        public void OnException_ShouldCallLocalizerWithInternalServerErrorKey()
        {
            // Arrange
            var mockLocalizer = new Mock<IErrorLocalizer>();
            mockLocalizer.Setup(m => m.Get(ErrorMessageKeys.InternalServerError))
                .Returns(DefaultErrorMessages.InternalServerError);

            var filter = CreateFilter(mockLocalizer.Object);
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var exceptionContext = new ExceptionContext(
                new ActionContext(httpContext, new RouteData(), actionDescriptor),
                [])
            {
                Exception = new Exception("Test")
            };

            // Act
            filter.OnException(exceptionContext);

            // Assert
            mockLocalizer.Verify(m => m.Get(ErrorMessageKeys.InternalServerError), Times.Once);
        }

        [Fact]
        public void OnException_ShouldSetLocalizedMessageAsTitle()
        {
            // Arrange
            var expectedMessage = "An unexpected error occurred.";
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.InternalServerError) == expectedMessage);

            var filter = CreateFilter(mockLocalizer);
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var exceptionContext = new ExceptionContext(
                new ActionContext(httpContext, new RouteData(), actionDescriptor),
                [])
            {
                Exception = new Exception("Test")
            };

            // Act
            filter.OnException(exceptionContext);

            // Assert
            var errorResult = exceptionContext.Result as ErrorResult;
            var errorDetails = errorResult?.Value as ErrorDetails;
            errorDetails?.Title.ShouldBe(expectedMessage);
        }

        [Fact]
        public void OnException_WithNullLocalizer_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new ExceptionFilter(null!, Microsoft.Extensions.Options.Options.Create(new ErrorHandlingOptions())));
        }

        [Fact]
        public void OnException_WithNullOptions_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new ExceptionFilter(Mock.Of<IErrorLocalizer>(), null!));
        }
    }
}
