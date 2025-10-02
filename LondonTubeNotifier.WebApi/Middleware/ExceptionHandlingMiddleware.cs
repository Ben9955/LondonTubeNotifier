using System.ComponentModel.DataAnnotations;
using LondonTubeNotifier.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LondonTubeNotifier.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next, IHostEnvironment env)
        {
            _logger = logger;
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException ex)
            {
                _logger.LogInformation(ex, "Validation failed: {Message}", ex.Message);
                await WriteProblemDetails(context, 400, "Validation failed", ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Model validation failed: {Message}", ex.Message);
                await WriteProblemDetails(context, 400, "Invalid request", ex.Message);
            }
            catch (DomainNotFoundException ex)
            {
                _logger.LogInformation(ex, "Resource not found: {Message}", ex.Message);
                await WriteProblemDetails(context, 404, "Resource not found", ex.Message);
            }
            catch (EntityUpdateException ex)
            {
                _logger.LogError(ex, "Entity update failed: {Message}", ex.Message);
                await WriteProblemDetails(context, 500, "An unexpected error occurred.", "An unexpected error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
                var detail = _env.IsDevelopment() ? ex.ToString() : "Please contact support.";
                await WriteProblemDetails(context, 500, "An unexpected error occurred.", detail);
            }
        }

        private static async Task WriteProblemDetails(HttpContext context, int statusCode, string title, string detail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
