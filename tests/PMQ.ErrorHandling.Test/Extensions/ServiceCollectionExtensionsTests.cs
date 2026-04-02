namespace PMQ.ErrorHandling.Test.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddErrorHandling_WithoutOptions_ShouldRegisterDefaultServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IErrorLocalizer>().ShouldNotBeNull();
        serviceProvider.GetService<ExceptionFilter>().ShouldNotBeNull();
        serviceProvider.GetService<NotificationFilter>().ShouldNotBeNull();
    }

    [Fact]
    public void AddErrorHandling_WithoutOptions_ShouldRegisterErrorLocalizerAsScopedService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var localizer1 = serviceProvider.CreateScope().ServiceProvider.GetService<IErrorLocalizer>();
        var localizer2 = serviceProvider.CreateScope().ServiceProvider.GetService<IErrorLocalizer>();

        localizer1.ShouldNotBeSameAs(localizer2);
    }

    [Fact]
    public void AddErrorHandling_WithOptions_ShouldApplyCustomCulture()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling(options => options.Culture = "pt-BR");
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.Culture.ShouldBe("pt-BR");
    }

    [Fact]
    public void AddErrorHandling_WithOptions_ShouldApplyIncludeExceptionDetails()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling(options => options.IncludeExceptionDetails = false);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.IncludeExceptionDetails.ShouldBeFalse();
    }

    [Fact]
    public void AddErrorHandling_WithOptions_ShouldApplyIncludeTraceId()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling(options => options.IncludeTraceId = false);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.IncludeTraceId.ShouldBeFalse();
    }

    [Fact]
    public void AddErrorHandling_WithOptions_ShouldApplyCustomMessages()
    {
        // Arrange
        var services = new ServiceCollection();
        var customMessages = new Dictionary<string, string>
        {
            { "InternalServerError", "Custom Error Message" }
        };

        // Act
        services.AddErrorHandling(options => options.CustomMessages = customMessages);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.CustomMessages.ShouldContainKey("InternalServerError");
        optionsSnapshot.CurrentValue.CustomMessages["InternalServerError"].ShouldBe("Custom Error Message");
    }

    [Fact]
    public void AddErrorHandling_ShouldReturnServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddErrorHandling();

        // Assert
        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddErrorHandling_WithOptions_ShouldReturnServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddErrorHandling(options => { });

        // Assert
        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddErrorHandling_ShouldAllowMultipleConfigurations()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling(options => options.Culture = "pt-BR");
        services.AddErrorHandling(options => options.IncludeExceptionDetails = false);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.Culture.ShouldBe("pt-BR");
        optionsSnapshot.CurrentValue.IncludeExceptionDetails.ShouldBeFalse();
    }

    [Fact]
    public void AddErrorHandling_DefaultOptions_ShouldSetEnglishCulture()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.Culture.ShouldBe("en-US");
    }

    [Fact]
    public void AddErrorHandling_DefaultOptions_ShouldIncludeExceptionDetails()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.IncludeExceptionDetails.ShouldBeTrue();
    }

    [Fact]
    public void AddErrorHandling_DefaultOptions_ShouldIncludeTraceId()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrorHandling();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsMonitor<ErrorHandlingOptions>>();
        optionsSnapshot.CurrentValue.IncludeTraceId.ShouldBeTrue();
    }
}
