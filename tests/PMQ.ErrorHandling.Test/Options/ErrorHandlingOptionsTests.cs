namespace PMQ.ErrorHandling.Test.Options;

public class ErrorHandlingOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var options = new ErrorHandlingOptions();

        // Assert
        options.IncludeExceptionDetails.ShouldBeTrue();
        options.IncludeTraceId.ShouldBeTrue();
        options.Culture.ShouldBe("en-US");
        options.CustomMessages.ShouldNotBeNull();
        options.CustomMessages.ShouldBeEmpty();
    }

    [Fact]
    public void IncludeExceptionDetails_ShouldBeSettable()
    {
        // Arrange
        var options = new ErrorHandlingOptions
        {
            // Act
            IncludeExceptionDetails = false
        };

        // Assert
        options.IncludeExceptionDetails.ShouldBeFalse();
    }

    [Fact]
    public void IncludeTraceId_ShouldBeSettable()
    {
        // Arrange
        var options = new ErrorHandlingOptions
        {
            // Act
            IncludeTraceId = false
        };

        // Assert
        options.IncludeTraceId.ShouldBeFalse();
    }

    [Fact]
    public void Culture_ShouldBeSettable()
    {
        // Arrange
        var options = new ErrorHandlingOptions
        {
            // Act
            Culture = "pt-BR"
        };

        // Assert
        options.Culture.ShouldBe("pt-BR");
    }

    [Fact]
    public void CustomMessages_ShouldBeSettable()
    {
        // Arrange
        var options = new ErrorHandlingOptions();

        var customMessages = new Dictionary<string, string>
        {
            { "TestKey", "Custom Message" }
        };

        // Act
        options.CustomMessages = customMessages;

        // Assert
        options.CustomMessages.ShouldBeSameAs(customMessages);
        options.CustomMessages.Count.ShouldBe(1);
    }

    [Fact]
    public void CustomMessages_ShouldAllowAddingMessages()
    {
        // Arrange
        var options = new ErrorHandlingOptions();

        // Act
        options.CustomMessages.Add("ErrorKey", "Error Message");

        // Assert
        options.CustomMessages.ShouldContainKey("ErrorKey");
        options.CustomMessages["ErrorKey"].ShouldBe("Error Message");
    }
}
