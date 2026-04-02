using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace PMQ.ErrorHandling.Test.Filters
{
    public class NotificationFilterTests
    {
        private static NotificationFilter CreateFilter(
            INotificationContext? context = null,
            IErrorLocalizer? localizer = null)
        {
            context ??= Mock.Of<INotificationContext>(m => m.HasNotifications == false);
            localizer ??= Mock.Of<IErrorLocalizer>(m => m.Get(It.IsAny<string>()) == "Test Message");

            return new NotificationFilter(context, localizer);
        }

        private static ActionExecutedContext CreateActionExecutedContext()
        {
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                actionDescriptor);

            return new ActionExecutedContext(actionContext, [], controller: null);
        }

        [Fact]
        public void OnActionExecuted_WithNoNotifications_ShouldNotSetResult()
        {
            // Arrange
            var mockContext = Mock.Of<INotificationContext>(m => m.HasNotifications == false);
            var filter = CreateFilter(mockContext);
            var context = CreateActionExecutedContext();
            context.Result = new OkResult();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void OnActionExecuted_WithValidationNotification_ShouldReturnErrorResultWith400Status()
        {
            // Arrange
            var notification = new Notifications.Notification("Field is required", "Name", NotificationType.Validation);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.ValidationError) == "Validation error occurred");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            errorResult?.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public void OnActionExecuted_WithNotFoundNotification_ShouldReturnErrorResultWith404Status()
        {
            // Arrange
            var notification = new Notifications.Notification("Resource not found", "Id", NotificationType.NotFound);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.NotFound) == "Not found");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            errorResult?.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        [Fact]
        public void OnActionExecuted_WithAccessDeniedNotification_ShouldReturnErrorResultWith403Status()
        {
            // Arrange
            var notification = new Notifications.Notification("Access denied", "Permission", NotificationType.AccessDenied);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.AccessDenied) == "Access denied");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            errorResult?.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        }

        [Fact]
        public void OnActionExecuted_WithInconsistentStateNotification_ShouldReturnErrorResultWith409Status()
        {
            // Arrange
            var notification = new Notifications.Notification("Inconsistent state", "Data", NotificationType.InconsistentState);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.InconsistentState) == "Inconsistent state");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            errorResult?.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
        }

        [Fact]
        public void OnActionExecuted_WithBusinessRuleNotification_ShouldReturnErrorResultWith422Status()
        {
            // Arrange
            var notification = new Notifications.Notification("Business rule violated", "Rule", NotificationType.BusinessRule);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.BusinessRule) == "Business rule failed");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            errorResult?.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        }

        [Fact]
        public void OnActionExecuted_WithMultipleNotifications_ShouldUseFirstNotificationType()
        {
            // Arrange
            var notifications = new[]
            {
                new Notifications.Notification("First", "Field1", NotificationType.Validation),
                new Notifications.Notification("Second", "Field2", NotificationType.NotFound)
            };

            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.ValidationError) == "Validation error");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            errorResult?.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public void OnActionExecuted_ShouldIncludeErrorsInResponse()
        {
            // Arrange
            var notification = new Notifications.Notification("Error message", "FieldName", NotificationType.Validation);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(It.IsAny<string>()) == "Test");

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            var errorDetails = errorResult?.Value as ErrorDetails;
            errorDetails?.Errors.ShouldNotBeNull();
            errorDetails?.Errors?.ShouldContain(e => e.Message == "Error message");
        }

        [Fact]
        public void OnActionExecuted_ShouldSetLocalizedTitle()
        {
            // Arrange
            var notification = new Notifications.Notification("Error", "Field", NotificationType.NotFound);
            var notifications = new[] { notification };
            var mockContext = new Mock<INotificationContext>();

            mockContext.Setup(m => m.HasNotifications).Returns(true);
            mockContext.Setup(m => m.Notifications).Returns(notifications);
        
            var expectedTitle = "The requested resource was not found";
            var mockLocalizer = Mock.Of<IErrorLocalizer>(m =>
                m.Get(ErrorMessageKeys.NotFound) == expectedTitle);

            var filter = new NotificationFilter(mockContext.Object, mockLocalizer);
            var context = CreateActionExecutedContext();

            // Act
            filter.OnActionExecuted(context);

            // Assert
            context.Result.ShouldBeOfType<ErrorResult>();
            var errorResult = context.Result as ErrorResult;
            var errorDetails = errorResult?.Value as ErrorDetails;
            errorDetails?.Title.ShouldBe(expectedTitle);
        }

        [Fact]
        public void OnActionExecuted_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new NotificationFilter(context: null!, Mock.Of<IErrorLocalizer>()));
        }

        [Fact]
        public void OnActionExecuted_WithNullLocalizer_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new NotificationFilter(Mock.Of<INotificationContext>(), localizer: null!));
        }

        [Fact]
        public void OnActionExecuting_ShouldDoNothing()
        {
            // Arrange
            var filter = CreateFilter();
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                actionDescriptor);

            var context = new ActionExecutingContext(
                actionContext,
                [],
                new Dictionary<string, object?>(),
                controller: null);

            var originalResult = context.Result;

            // Act
            filter.OnActionExecuting(context);

            // Assert
            context.Result.ShouldBe(originalResult);
        }
    }
}