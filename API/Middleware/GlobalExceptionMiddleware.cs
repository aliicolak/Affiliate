using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace API.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Converts exceptions to appropriate HTTP responses with ProblemDetails format.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validation Failed",
                string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))
            ),
            InvalidOperationException invalidOpEx => (
                HttpStatusCode.BadRequest,
                "Invalid Operation",
                invalidOpEx.Message
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action."
            ),
            KeyNotFoundException keyNotFoundEx => (
                HttpStatusCode.NotFound,
                "Not Found",
                keyNotFoundEx.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment() ? exception.ToString() : "An unexpected error occurred."
            )
        };

        _logger.LogError(
            exception,
            "Exception occurred: {Message} | Path: {Path} | StatusCode: {StatusCode}",
            exception.Message,
            context.Request.Path,
            (int)statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (exception is ValidationException valEx)
        {
            problemDetails.Extensions["errors"] = valEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
