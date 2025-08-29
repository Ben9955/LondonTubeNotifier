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

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next, IHostEnvironment inv)
        {
            _logger = logger;
            _next = next;
            _env = inv;
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

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = 400,
                    Title = "Validation failed",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                });
            }
            catch (DomainNotFoundException ex)
            {
                _logger.LogInformation(ex, "Resource not found: {Message}", ex.Message);

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = 404,
                    Title = "Resource not found",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                });
            }
            catch (EntityUpdateException ex)
            {
                _logger.LogError(ex, "Entity update failed: {Message}", ex.Message);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = 500,
                    Title = "An unexpected error occurred.",
                    Detail = "An unexpected error occurred. Please try again.",
                    Instance = context.Request.Path
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = _env.IsDevelopment() ? ex.ToString() : "Please contact support.",
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
