using FluentValidation;
using GenshinAccountAnalyzer.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GenshinAccountAnalyzer.Api.Middleware;

/// <summary>
/// Converts known application exceptions into RFC 7807 <see cref="ProblemDetails"/> responses so the
/// API never leaks stack traces and always returns a predictable error shape.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>Initializes the middleware.</summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware, translating handled exceptions into problem responses.</summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            _logger.LogWarning(exception, "Request validation failed.");
            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation failed",
                string.Join(" ", exception.Errors.Select(error => error.ErrorMessage))).ConfigureAwait(false);
        }
        catch (AccountImportException exception)
        {
            _logger.LogWarning(exception, "Account import failed.");
            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Import failed",
                exception.Message).ConfigureAwait(false);
        }
    }

    private static Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
        };

        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(problem);
    }
}
