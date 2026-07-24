using DotNetApiStarterKit.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DotNetApiStarterKit.Api.Controllers
{
    namespace StudentEventRegistrationPortal.API.Controllers
    {
        [ApiExplorerSettings(IgnoreApi = true)] // keep this out of Swagger's own doc listing
        public class SwaggerLoginController : Controller
        {
            private readonly IConfiguration _configuration;

            public SwaggerLoginController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpGet("/swagger-login")]
            public IActionResult Get(string? returnUrl, bool failed = false)
            {
                var errorHtml = failed
                    ? "<p style='color:#c0392b;margin:0 0 12px;'>Invalid username or password.</p>"
                    : "";

                var html = $@"
<!DOCTYPE html>
<html>
<head><title>Swagger Login</title></head>
<body style='font-family:Arial,sans-serif;max-width:360px;margin:80px auto;padding:24px;border:1px solid #ddd;border-radius:8px;'>
    <h2 style='margin-top:0;'>Swagger Access</h2>
    {errorHtml}
    <form method='post' action='/swagger-login'>
        <input type='hidden' name='returnUrl' value='{System.Net.WebUtility.HtmlEncode(returnUrl ?? "/swagger")}' />
        <div style='margin-bottom:12px;'>
            <label>Username</label><br/>
            <input type='text' name='username' style='width:100%;padding:8px;' autofocus />
        </div>
        <div style='margin-bottom:16px;'>
            <label>Password</label><br/>
            <input type='password' name='password' style='width:100%;padding:8px;' />
        </div>
        <button type='submit' style='width:100%;padding:10px;background:#5B4FE8;color:#fff;border:none;border-radius:4px;'>Sign In</button>
    </form>
</body>
</html>";

                return Content(html, "text/html");
            }

            [EnableRateLimiting("swagger-login-policy")]
            [HttpPost("/swagger-login")]
            public IActionResult Post(string username, string password, string? returnUrl)
            {
                var expectedUser = _configuration["Swagger:Username"];
                var expectedPassword = _configuration["Swagger:Password"];
                var signingKey = _configuration["Swagger:Key"];

                if (string.IsNullOrWhiteSpace(signingKey))
                {
                    return StatusCode(503, "Swagger is not configured.");
                }

                if (username != expectedUser || password != expectedPassword)
                {
                    var redirectUrl = $"/swagger-login?failed=true&returnUrl={Uri.EscapeDataString(returnUrl ?? "/swagger")}";
                    return Redirect(redirectUrl);
                }

                var cookieValue = SwaggerAuthMiddleware.CreateSessionCookieValue(signingKey);

                Response.Cookies.Append("swagger_session", cookieValue, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict
                    // No Expires/MaxAge set here on purpose — cookie itself is a
                    // session cookie (cleared when the browser fully closes), and
                    // the signed payload additionally expires after 5 minutes
                    // regardless of whether the browser stays open.
                });

                return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/swagger" : returnUrl);
            }
        }
    }
}
