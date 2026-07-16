using System.Diagnostics;

namespace PromptlyNote.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = context.TraceIdentifier
        });

        var startTimestamp = Stopwatch.GetTimestamp();

        _logger.LogInformation("\n--> {Method} {Path} started\n",
            context.Request.Method, context.Request.Path);

        try
        {
            await _next(context);
        }
        finally
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

            _logger.LogInformation("\n<-- {Method} {Path} {StatusCode} in {ElapsedMs:0.00} ms\n",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsedMs);
        }
    }
}
