using System.Security.Cryptography;
using System.Text;

namespace SmartCollegeComplaintPortal.API.Middlewares
{
    public class SwaggerAuthMiddleware
    {
        private const string CookieName = "swagger_session";
        private const int SessionMinutes = 5;

        private readonly RequestDelegate _next;

        public SwaggerAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var path = context.Request.Path;

            if (!path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (path.StartsWithSegments("/swagger-login"))
            {
                await _next(context);
                return;
            }

            var signingKey = configuration["Swagger:Key"];
            if (string.IsNullOrWhiteSpace(signingKey))
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Swagger is not configured.");
                return;
            }

            var cookieValue = context.Request.Cookies[CookieName];

            if (IsValidSession(cookieValue, signingKey))
            {
                await _next(context);
                return;
            }

            var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
            context.Response.Redirect($"/swagger-login?returnUrl={returnUrl}");
        }

        public static bool IsValidSession(string? cookieValue, string signingKey)
        {
            if (string.IsNullOrWhiteSpace(cookieValue))
                return false;

            var parts = cookieValue.Split('.', 2);
            if (parts.Length != 2)
                return false;

            if (!long.TryParse(parts[0], out var expiryUnixSeconds))
                return false;

            var expectedSignature = Sign(parts[0], signingKey);
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedSignature),
                    Encoding.UTF8.GetBytes(parts[1])))
            {
                return false;
            }

            var expiry = DateTimeOffset.FromUnixTimeSeconds(expiryUnixSeconds);
            return expiry > DateTimeOffset.UtcNow;
        }

        public static string CreateSessionCookieValue(string signingKey)
        {
            var expiry = DateTimeOffset.UtcNow.AddMinutes(SessionMinutes).ToUnixTimeSeconds();
            var payload = expiry.ToString();
            var signature = Sign(payload, signingKey);
            return $"{payload}.{signature}";
        }

        public static int GetSessionMinutes() => SessionMinutes;

        private static string Sign(string payload, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(hash);
        }
    }
}