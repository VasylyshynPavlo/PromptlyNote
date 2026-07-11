using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Exceptions;
using System.Net;

namespace PromptlyNote.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, _logger);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger logger)
    {
        var (statusCode, detail) = exception switch
        {
            InternalException => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),

            ApiException apiEx => (apiEx.StatusCode, apiEx.Message),

            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized."),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request."),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        // 5xx — наші баги, повний стек на рівні Error.
        // 4xx від власних ApiException (NotFound/Forbidden/Conflict) — нормальний потік, не логуємо.
        // 4xx від "чужого" винятку (напр. ArgumentException) — несподіванка, лишаємо слід на Warning.
        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
        else if (exception is not ApiException)
            logger.LogWarning(exception, "Handled {StatusCode} from unexpected exception. TraceId: {TraceId}", statusCode, context.TraceIdentifier);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = GetTypeUri(statusCode),
            Title = GetTitle(statusCode),
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is NotFoundException notFoundException)
        {
            problem.Extensions["resource"] = notFoundException.Resource;
        }

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => ((HttpStatusCode)statusCode).ToString()
        };
    }

    private static string GetTypeUri(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest =>
            "https://www.rfc-editor.org/rfc/rfc9110#name-400-bad-request",

        StatusCodes.Status401Unauthorized =>
            "https://www.rfc-editor.org/rfc/rfc9110#name-401-unauthorized",

        StatusCodes.Status403Forbidden =>
            "https://www.rfc-editor.org/rfc/rfc9110#name-403-forbidden",

        StatusCodes.Status404NotFound =>
            "https://www.rfc-editor.org/rfc/rfc9110#name-404-not-found",

        StatusCodes.Status409Conflict =>
            "https://www.rfc-editor.org/rfc/rfc9110#name-409-conflict",

        StatusCodes.Status500InternalServerError =>
            "https://www.rfc-editor.org/rfc/rfc9110#name-500-internal-server-error",

        _ => "about:blank"
    };
}