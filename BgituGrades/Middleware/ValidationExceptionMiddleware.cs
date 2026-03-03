using FluentValidation;
using System.Net;
using System.Text.Json.Serialization;

namespace BgituGrades.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationExceptionMiddleware> _logger;

        public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
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
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation exception occurred");
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new ValidationErrorResponse
            {
                Errors = exception.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            };

            return context.Response.WriteAsJsonAsync(response);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ErrorResponse
            {
                Message = "An internal server error occurred",
                Detail = exception.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    public class ValidationErrorResponse
    {
        [JsonPropertyName("errors")]
        public Dictionary<string, string[]> Errors { get; set; } = new();

        [JsonPropertyName("type")]
        public string Type { get; } = "https://api.example.com/docs/errors#validation-error";

        [JsonPropertyName("title")]
        public string Title { get; } = "One or more validation errors occurred";
    }

    public class ErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;
    }
}
