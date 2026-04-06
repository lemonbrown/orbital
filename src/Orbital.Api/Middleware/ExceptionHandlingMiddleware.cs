using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Orbital.Application.Common.Exceptions;

namespace Orbital.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            DomainException de => (StatusCodes.Status422UnprocessableEntity, "Domain Rule Violation", de.Message),
            NotFoundException nfe => (StatusCodes.Status404NotFound, "Not Found", nfe.Message),
            UnauthorizedException ue => (StatusCodes.Status401Unauthorized, "Unauthorized", ue.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
