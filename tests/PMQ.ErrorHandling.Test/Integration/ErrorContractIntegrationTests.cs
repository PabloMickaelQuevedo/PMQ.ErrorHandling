using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PMQ.ErrorHandling.Test.Integration;

/// <summary>
/// Runs a real ASP.NET Core pipeline and checks the error contract end to end: what the client
/// receives and what operations can see. Each test pins down a behavior that unit tests on the
/// filters cannot, because it depends on which component writes the response.
/// </summary>
public sealed class ErrorContractIntegrationTests
{
    private const string ProblemJson = "application/problem+json";
    private const string Secret = "Server=db;Password=secret";

    [Fact]
    public async Task UnhandledException_InAction_ReturnsLocalizedContractAndIsLogged()
    {
        // Arrange
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.GetAsync("/probe/throw");

        // Assert — the contract
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(ProblemJson);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.InternalServerError);
        json.TryGetProperty("type", out _).ShouldBeTrue();
        json.TryGetProperty("traceId", out _).ShouldBeTrue();
        json.TryGetProperty("detail", out _).ShouldBeFalse();

        // Assert — operations can see it. An MVC exception filter that handles the exception
        // suppresses this log entirely; that was the 1.1.x behavior.
        api.Logs.ShouldContain(l => l.Level == LogLevel.Error && l.Exception is InvalidOperationException);
    }

    [Fact]
    public async Task UnhandledException_WithIncludeExceptionDetails_ExposesMessage()
    {
        // Arrange — controller endpoints are written by MVC's problem details writer, which does
        // not pass the exception to the customization. The message must still come through.
        await using var api = await TestApi.StartAsync(o => o.IncludeExceptionDetails = true);

        // Act
        var (_, json) = await api.GetAsync("/probe/throw");

        // Assert
        json.GetProperty("detail").GetString().ShouldBe(Secret);
    }

    [Fact]
    public async Task UnhandledException_OutsideMvc_WithoutExplicitUseExceptionHandler_IsHandled()
    {
        // Arrange — the application never calls UseExceptionHandler(); AddErrorHandling() alone
        // must cover exceptions anywhere in the pipeline, not only in actions.
        await using var api = await TestApi.StartAsync(explicitExceptionHandler: false);

        // Act
        var (response, json) = await api.GetAsync("/boom-middleware");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(ProblemJson);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.InternalServerError);
        api.Logs.ShouldContain(l => l.Level == LogLevel.Error && l.Exception is InvalidOperationException);
    }

    [Fact]
    public async Task Notification_OnControllerProducingJson_ReturnsProblemJsonWithErrors()
    {
        // Arrange
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.GetAsync("/probe/notification");

        // Assert — [Produces("application/json")] on the controller no longer changes the media
        // type, and the members declared on ErrorDetails survive the framework's writer.
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(ProblemJson);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.NotFound);
        json.TryGetProperty("type", out _).ShouldBeTrue();
        json.TryGetProperty("traceId", out _).ShouldBeTrue();

        var error = json.GetProperty("errors").EnumerateArray().ShouldHaveSingleItem();
        error.GetProperty("field").GetString().ShouldBe("Id");
        error.GetProperty("message").GetString().ShouldBe("Exemplo não encontrado.");
    }

    [Fact]
    public async Task ErrorResultFrom_KeepsItsOwnTitle()
    {
        // Arrange — MVC's writer builds new problem details and customizes those, so a check on
        // the object passed in cannot tell that this title was chosen on purpose.
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.GetAsync("/probe/custom-title");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        json.GetProperty("title").GetString().ShouldBe("Título próprio");
    }

    [Fact]
    public async Task UntypedNotification_KeepsValidationTitleOn422()
    {
        // Arrange — a notification without a type maps to 422 with the validation title, which is
        // not the title the 422 status maps to on its own.
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.GetAsync("/probe/untyped-notification");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.ValidationError);
    }

    [Fact]
    public async Task InvalidModelState_ReturnsProblemJsonWithErrors()
    {
        // Arrange
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.PostAsync("/probe/body", "{ not json");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(ProblemJson);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.ValidationError);
        json.GetProperty("errors").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task UnmatchedRoute_WithStatusCodePages_ReturnsSameContract()
    {
        // Arrange
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.GetAsync("/does-not-exist");

        // Assert — a 404 written by the framework reads the same as one from a notification.
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(ProblemJson);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.NotFound);
        json.TryGetProperty("traceId", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task MethodNotAllowed_WithStatusCodePages_IsLocalized()
    {
        // Arrange
        await using var api = await TestApi.StartAsync();

        // Act
        var (response, json) = await api.SendAsync(HttpMethod.Delete, "/probe/notification");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.MethodNotAllowed);
    }

    [Fact]
    public async Task IncludeTraceIdFalse_OmitsTraceIdOnEveryPath()
    {
        // Arrange
        await using var api = await TestApi.StartAsync(o => o.IncludeTraceId = false);

        // Act
        var (_, fromException) = await api.GetAsync("/probe/throw");
        var (_, fromNotification) = await api.GetAsync("/probe/notification");
        var (_, fromStatusCodePages) = await api.GetAsync("/does-not-exist");

        // Assert
        fromException.TryGetProperty("traceId", out _).ShouldBeFalse();
        fromNotification.TryGetProperty("traceId", out _).ShouldBeFalse();
        fromStatusCodePages.TryGetProperty("traceId", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ApplicationCustomizeProblemDetails_StillRunsAfterTheContract()
    {
        // Arrange — an application customization configured before AddErrorHandling must not be
        // overwritten, and it runs last so it can adjust anything.
        await using var api = await TestApi.StartAsync(
            problemDetails: o => o.CustomizeProblemDetails = c => c.ProblemDetails.Extensions["tenant"] = "acme");

        // Act
        var (_, json) = await api.GetAsync("/probe/throw");

        // Assert
        json.GetProperty("tenant").GetString().ShouldBe("acme");
        json.GetProperty("title").GetString().ShouldBe(PortugueseBRErrorMessages.InternalServerError);
    }

    public sealed record LogEntry(string Category, LogLevel Level, Exception? Exception);

    private sealed class TestApi : IAsyncDisposable
    {
        private readonly WebApplication _app;
        private readonly HttpClient _client;

        private TestApi(WebApplication app, ConcurrentQueue<LogEntry> logs)
        {
            _app = app;
            _client = app.GetTestClient();
            LogQueue = logs;
        }

        private ConcurrentQueue<LogEntry> LogQueue { get; }

        public IReadOnlyCollection<LogEntry> Logs => LogQueue.ToArray();

        public static async Task<TestApi> StartAsync(
            Action<ErrorHandlingOptions>? configure = null,
            bool explicitExceptionHandler = true,
            Action<Microsoft.AspNetCore.Http.ProblemDetailsOptions>? problemDetails = null)
        {
            var logs = new ConcurrentQueue<LogEntry>();

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new CapturingLoggerProvider(logs));

            builder.Services.AddControllers().AddApplicationPart(typeof(ProbeController).Assembly);
            if (problemDetails is not null)
                builder.Services.AddProblemDetails(problemDetails);

            builder.Services.AddErrorHandling(o =>
            {
                o.Culture = "pt-BR";
                configure?.Invoke(o);
            });

            var app = builder.Build();

            if (explicitExceptionHandler)
                app.UseExceptionHandler();

            app.UseStatusCodePages();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/boom-middleware")
                    throw new InvalidOperationException(Secret);
                await next();
            });
            app.MapControllers();

            await app.StartAsync();
            return new TestApi(app, logs);
        }

        public Task<(HttpResponseMessage Response, JsonElement Json)> GetAsync(string path) =>
            SendAsync(HttpMethod.Get, path);

        public async Task<(HttpResponseMessage Response, JsonElement Json)> PostAsync(string path, string body)
        {
            using var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(path, content);
            return (response, await ReadAsync(response));
        }

        public async Task<(HttpResponseMessage Response, JsonElement Json)> SendAsync(HttpMethod method, string path)
        {
            using var request = new HttpRequestMessage(method, path);
            var response = await _client.SendAsync(request);
            return (response, await ReadAsync(response));
        }

        private static async Task<JsonElement> ReadAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(body).RootElement.Clone();
        }

        public async ValueTask DisposeAsync()
        {
            _client.Dispose();
            await _app.DisposeAsync();
        }
    }

    private sealed class CapturingLoggerProvider(ConcurrentQueue<LogEntry> sink) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new CapturingLogger(categoryName, sink);
        public void Dispose() { }

        private sealed class CapturingLogger(string category, ConcurrentQueue<LogEntry> sink) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                sink.Enqueue(new LogEntry(category, logLevel, exception));
        }
    }
}

[ApiController]
[Route("probe")]
[Produces("application/json")]
public sealed class ProbeController(INotificationContext notifications) : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult Throw() => throw new InvalidOperationException("Server=db;Password=secret");

    [HttpGet("notification")]
    public IActionResult Notification()
    {
        notifications.Add("Id", "Exemplo não encontrado.", NotificationType.NotFound);
        return Ok(null);
    }

    [HttpGet("custom-title")]
    public IActionResult CustomTitle() => ErrorResult.From("Título próprio", StatusCodes.Status422UnprocessableEntity);

    [HttpGet("untyped-notification")]
    public IActionResult UntypedNotification()
    {
        notifications.Add(new PMQ.Notifications.Notification("Nome", "Nome inválido."));
        return Ok(null);
    }

    [HttpPost("body")]
    public IActionResult Body(Payload payload) => Ok(payload);

    public sealed record Payload(string Name);
}
