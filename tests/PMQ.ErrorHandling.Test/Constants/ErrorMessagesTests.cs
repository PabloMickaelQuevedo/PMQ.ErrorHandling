namespace PMQ.ErrorHandling.Test.Constants;

public class ErrorMessageKeysTests
{
    [Fact]
    public void InternalServerError_ShouldHaveCorrectValue()
    {
        // Arrange & Act & Assert
        ErrorMessageKeys.InternalServerError.ShouldBe("InternalServerError");
    }

    [Fact]
    public void ValidationError_ShouldHaveCorrectValue()
    {
        // Arrange & Act & Assert
        ErrorMessageKeys.ValidationError.ShouldBe("ValidationError");
    }

    [Fact]
    public void NotFound_ShouldHaveCorrectValue()
    {
        // Arrange & Act & Assert
        ErrorMessageKeys.NotFound.ShouldBe("NotFound");
    }

    [Fact]
    public void AccessDenied_ShouldHaveCorrectValue()
    {
        // Arrange & Act & Assert
        ErrorMessageKeys.AccessDenied.ShouldBe("AccessDenied");
    }

    [Fact]
    public void InconsistentState_ShouldHaveCorrectValue()
    {
        // Arrange & Act & Assert
        ErrorMessageKeys.InconsistentState.ShouldBe("InconsistentState");
    }

    [Fact]
    public void BusinessRule_ShouldHaveCorrectValue()
    {
        // Arrange & Act & Assert
        ErrorMessageKeys.BusinessRule.ShouldBe("BusinessRule");
    }
}

public class DefaultErrorMessagesTests
{
    [Fact]
    public void InternalServerError_ShouldHaveCorrectMessage()
    {
        // Arrange & Act & Assert
        DefaultErrorMessages.InternalServerError.ShouldBe("An unexpected error occurred.");
    }

    [Fact]
    public void ValidationError_ShouldHaveCorrectMessage()
    {
        // Arrange & Act & Assert
        DefaultErrorMessages.ValidationError.ShouldBe("One or more validation errors occurred.");
    }

    [Fact]
    public void NotFound_ShouldHaveCorrectMessage()
    {
        // Arrange & Act & Assert
        DefaultErrorMessages.NotFound.ShouldBe("Resource not found.");
    }

    [Fact]
    public void AccessDenied_ShouldHaveCorrectMessage()
    {
        // Arrange & Act & Assert
        DefaultErrorMessages.AccessDenied.ShouldBe("Access denied.");
    }

    [Fact]
    public void InconsistentState_ShouldHaveCorrectMessage()
    {
        // Arrange & Act & Assert
        DefaultErrorMessages.InconsistentState.ShouldBe("Operation resulted in an inconsistent state.");
    }

    [Fact]
    public void BusinessRule_ShouldHaveCorrectMessage()
    {
        // Arrange & Act & Assert
        DefaultErrorMessages.BusinessRule.ShouldBe("A business rule validation failed.");
    }
}

public class PortugueseBRErrorMessagesTests
{
    [Fact]
    public void InternalServerError_ShouldHaveCorrectPortugueseMessage()
    {
        // Arrange & Act & Assert
        PortugueseBRErrorMessages.InternalServerError.ShouldBe("Um erro inesperado ocorreu.");
    }

    [Fact]
    public void ValidationError_ShouldHaveCorrectPortugueseMessage()
    {
        // Arrange & Act & Assert
        PortugueseBRErrorMessages.ValidationError.ShouldBe("Um ou mais erros de validação ocorreram.");
    }

    [Fact]
    public void NotFound_ShouldHaveCorrectPortugueseMessage()
    {
        // Arrange & Act & Assert
        PortugueseBRErrorMessages.NotFound.ShouldBe("Recurso não encontrado.");
    }

    [Fact]
    public void AccessDenied_ShouldHaveCorrectPortugueseMessage()
    {
        // Arrange & Act & Assert
        PortugueseBRErrorMessages.AccessDenied.ShouldBe("Acesso negado.");
    }

    [Fact]
    public void InconsistentState_ShouldHaveCorrectPortugueseMessage()
    {
        // Arrange & Act & Assert
        PortugueseBRErrorMessages.InconsistentState.ShouldBe("A operação resultou em um estado inconsistente.");
    }

    [Fact]
    public void BusinessRule_ShouldHaveCorrectPortugueseMessage()
    {
        // Arrange & Act & Assert
        PortugueseBRErrorMessages.BusinessRule.ShouldBe("Uma validação de regra de negócio falhou.");
    }
}
