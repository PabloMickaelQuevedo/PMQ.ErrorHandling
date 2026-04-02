namespace PMQ.ErrorHandling.Test.Localization;

public class DefaultErrorLocalizerTests
{
    private readonly AutoMocker _mocker = new();

    private IErrorLocalizer CreateLocalizer(ErrorHandlingOptions? options = null)
    {
        options ??= new ErrorHandlingOptions();
        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(options);
        return new DefaultErrorLocalizer(optionsWrapper);
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new DefaultErrorLocalizer(null!));
    }

    [Fact]
    public void Get_WithInternalServerErrorKey_EnglishCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.InternalServerError);

        // Assert
        result.ShouldBe(DefaultErrorMessages.InternalServerError);
    }

    [Fact]
    public void Get_WithValidationErrorKey_EnglishCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.ValidationError);

        // Assert
        result.ShouldBe(DefaultErrorMessages.ValidationError);
    }

    [Fact]
    public void Get_WithNotFoundKey_EnglishCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.NotFound);

        // Assert
        result.ShouldBe(DefaultErrorMessages.NotFound);
    }

    [Fact]
    public void Get_WithAccessDeniedKey_EnglishCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.AccessDenied);

        // Assert
        result.ShouldBe(DefaultErrorMessages.AccessDenied);
    }

    [Fact]
    public void Get_WithInconsistentStateKey_EnglishCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.InconsistentState);

        // Assert
        result.ShouldBe(DefaultErrorMessages.InconsistentState);
    }

    [Fact]
    public void Get_WithBusinessRuleKey_EnglishCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.BusinessRule);

        // Assert
        result.ShouldBe(DefaultErrorMessages.BusinessRule);
    }

    [Fact]
    public void Get_WithInternalServerErrorKey_PortugueseCulture_ShouldReturnPortugueseMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "pt-BR" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.InternalServerError);

        // Assert
        result.ShouldBe(PortugueseBRErrorMessages.InternalServerError);
    }

    [Fact]
    public void Get_WithValidationErrorKey_PortugueseCulture_ShouldReturnPortugueseMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "pt-BR" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.ValidationError);

        // Assert
        result.ShouldBe(PortugueseBRErrorMessages.ValidationError);
    }

    [Fact]
    public void Get_WithNotFoundKey_PortugueseCulture_ShouldReturnPortugueseMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "pt-BR" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.NotFound);

        // Assert
        result.ShouldBe(PortugueseBRErrorMessages.NotFound);
    }

    [Fact]
    public void Get_WithCustomMessage_ShouldReturnCustomMessage()
    {
        // Arrange
        var customMessages = new Dictionary<string, string>
        {
            { ErrorMessageKeys.InternalServerError, "Custom error message" }
        };

        var options = new ErrorHandlingOptions { CustomMessages = customMessages };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.InternalServerError);

        // Assert
        result.ShouldBe("Custom error message");
    }

    [Fact]
    public void Get_WithCustomMessage_ShouldTakePrecedenceOverDefault()
    {
        // Arrange
        var customMessages = new Dictionary<string, string>
        {
            { ErrorMessageKeys.ValidationError, "My custom validation error" }
        };

        var options = new ErrorHandlingOptions 
        { 
            Culture = "en-US",
            CustomMessages = customMessages 
        };

        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.ValidationError);

        // Assert
        result.ShouldBe("My custom validation error");
        result.ShouldNotBe(DefaultErrorMessages.ValidationError);
    }

    [Fact]
    public void Get_WithUnknownKey_ShouldReturnKeyItself()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "en-US" };
        var localizer = CreateLocalizer(options);
        var unknownKey = "UnknownErrorKey";

        // Act
        var result = localizer.Get(unknownKey);

        // Assert
        result.ShouldBe(unknownKey);
    }

    [Fact]
    public void Get_WithPortugueseStartingCulture_ShouldReturnPortugueseMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "pt" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.BusinessRule);

        // Assert
        result.ShouldBe(PortugueseBRErrorMessages.BusinessRule);
    }

    [Fact]
    public void Get_WithPT_PTCulture_ShouldReturnPortugueseMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "pt-PT" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.InconsistentState);

        // Assert
        result.ShouldBe(PortugueseBRErrorMessages.InconsistentState);
    }

    [Fact]
    public void Get_WithNullCulture_ShouldDefaultToEnglish()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = null! };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.NotFound);

        // Assert
        result.ShouldBe(DefaultErrorMessages.NotFound);
    }

    [Fact]
    public void Get_WithFrenchCulture_ShouldReturnEnglishMessage()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "fr-FR" };
        var localizer = CreateLocalizer(options);

        // Act
        var result = localizer.Get(ErrorMessageKeys.AccessDenied);

        // Assert
        result.ShouldBe(DefaultErrorMessages.AccessDenied);
    }

    [Fact]
    public void Get_MultipleCallsWithDifferentKeys_ShouldReturnCorrectMessages()
    {
        // Arrange
        var options = new ErrorHandlingOptions { Culture = "pt-BR" };
        var localizer = CreateLocalizer(options);

        // Act
        var result1 = localizer.Get(ErrorMessageKeys.ValidationError);
        var result2 = localizer.Get(ErrorMessageKeys.NotFound);
        var result3 = localizer.Get(ErrorMessageKeys.InternalServerError);

        // Assert
        result1.ShouldBe(PortugueseBRErrorMessages.ValidationError);
        result2.ShouldBe(PortugueseBRErrorMessages.NotFound);
        result3.ShouldBe(PortugueseBRErrorMessages.InternalServerError);
    }
}
