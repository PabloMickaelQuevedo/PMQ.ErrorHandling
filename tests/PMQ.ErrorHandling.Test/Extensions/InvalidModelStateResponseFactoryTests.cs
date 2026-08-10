using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace PMQ.ErrorHandling.Test.Extensions;

/// <summary>
/// Regressão da via de ModelState, que era a única a devolver a chave crua no Title e a
/// deixar o TraceId nulo, enquanto ExceptionFilter e NotificationFilter já devolviam o
/// título traduzido e o trace preenchido.
/// </summary>
public class InvalidModelStateResponseFactoryTests
{
    private static ActionContext CreateContextWithError(IServiceProvider services)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = services,
            TraceIdentifier = "trace-identifier-for-test",
        };

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "The Name field is required.");

        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), modelState);
    }

    private static (ApiBehaviorOptions Options, IServiceProvider Services) Configure(
        Action<ErrorHandlingOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddErrorHandling(configure ?? (_ => { }));

        var provider = services.BuildServiceProvider();

        return (provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value, provider);
    }

    private static ErrorDetails Invoke(Action<ErrorHandlingOptions>? configure = null)
    {
        var (options, services) = Configure(configure);
        var result = options.InvalidModelStateResponseFactory(CreateContextWithError(services));

        return result.ShouldBeOfType<ErrorResult>().Value.ShouldBeOfType<ErrorDetails>();
    }

    [Fact]
    public void InvalidModelState_ShouldReturnBadRequest()
    {
        Invoke().Status.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void InvalidModelState_ShouldLocalizeTitleInsteadOfReturningTheRawKey()
    {
        var error = Invoke();

        error.Title.ShouldNotBe(ErrorMessageKeys.ValidationError);
        error.Title.ShouldBe(DefaultErrorMessages.ValidationError);
    }

    [Fact]
    public void InvalidModelState_WithPortugueseCulture_ShouldLocalizeTitle()
    {
        var error = Invoke(options => options.Culture = "pt-BR");

        error.Title.ShouldBe(PortugueseBRErrorMessages.ValidationError);
    }

    [Fact]
    public void InvalidModelState_WithCustomMessage_ShouldPreferIt()
    {
        var error = Invoke(options =>
            options.CustomMessages[ErrorMessageKeys.ValidationError] = "Check your input.");

        error.Title.ShouldBe("Check your input.");
    }

    [Fact]
    public void InvalidModelState_ShouldPopulateTraceId()
    {
        Invoke().TraceId.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void InvalidModelState_ShouldMapModelErrorsToValidationErrors()
    {
        var error = Invoke();

        var validationError = error.Errors.ShouldNotBeNull().ShouldHaveSingleItem();
        validationError.Field.ShouldBe("Name");
        validationError.Message.ShouldBe("The Name field is required.");
    }
}
