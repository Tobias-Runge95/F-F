using System.Net;
using System.Text.Json;
using F_F.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException nf => (StatusCodes.Status404NotFound, nf.Message),
            InvalidCredentialsException ic => (StatusCodes.Status401Unauthorized, ic.Message),
            InvalidTokenException it => (StatusCodes.Status401Unauthorized, it.Message),
            TokenExpiredException te => (StatusCodes.Status401Unauthorized, te.Message),
            ArgumentException ae => (StatusCodes.Status400BadRequest, ae.Message),
            UnauthorizedAccessException ua => (StatusCodes.Status403Forbidden, ua.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception is InvalidOperationException or ArgumentException ? exception.Message : null,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}

