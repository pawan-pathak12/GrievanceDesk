using SmartCollegeComplaintPortal.API.Contracts;
using System.Text.Json;

namespace SmartCollegeComplaintPortal.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                UnauthorizedAccessException => (401, "You are not authorized to perform this action."),
                KeyNotFoundException => (404, ex.Message),
                ArgumentException => (400, ex.Message),
                FormatException => (400, "One or more values are not in the correct format."),
                InvalidDataException => (400, "The provided data is invalid."),

                OperationCanceledException => (408, "The request was cancelled."),
                //SqlException => (500, "A internal server error occurred."),
                InvalidOperationException => (500, "An internal server error occurred."),
                NotImplementedException => (501, "This feature is not yet available."),
                _ => (500, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            //  Same ErrorResponse record used in controllers — one consistent shape
            var response = new ErrorResponse(false, statusCode, null, new List<string> { message });

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
