using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.Domain.Constants;
using System.Threading.RateLimiting;

namespace SmartCollegeComplaintPortal.API.Extensions
{
    public static class RatelimitingExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services
             , IWebHostEnvironment env)
        {
            /* What does limit does : of login 
          * 
          * Window = 5 minutes, PermitLimit = 4

            12:00 → request 1 
            12:01 → request 2 
            12:02 → request 3 
            12:03 → request 4 
            12:04 → request 5  blocked
            12:05 → window resets → request 6  again
            After 5 minutes — counter resets to 0. Fresh 4 attempts allowed.
          */

            services.AddRateLimiter(options =>
            {
                // if enviroment is testing then limit is set to high
                bool isTesting = env.IsEnvironment("Test");
                options.AddFixedWindowLimiter("check-email", opt =>
             {
                 opt.PermitLimit = isTesting ? 1000 : 4;   //how many request are allowed in the time window
                 opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(5);
                 opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                 opt.QueueLimit = 0;
             });
                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = isTesting ? 1000 : 4;   //how many request are allowed in the time window
                    opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(5);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("otp", opt =>
                {
                    opt.PermitLimit = isTesting ? 1000 : 5;
                    opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(15);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });
                options.AddFixedWindowLimiter("verify-otp", opt =>
                {
                    opt.PermitLimit = isTesting ? 1000 : 5;
                    opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(5);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });
                options.AddFixedWindowLimiter("create-password", opt =>
                {
                    opt.PermitLimit = isTesting ? 1000 : 5;
                    opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(10);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("refresh", opt =>
                {
                    opt.PermitLimit = isTesting ? 1000 : 5;
                    opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(15);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("change-password", opt =>
                {
                    opt.PermitLimit = isTesting ? 1000 : 4;
                    opt.Window = isTesting ? TimeSpan.FromSeconds(1) : TimeSpan.FromDays(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;

                });

                //in aith controller 10 request per min 

                options.AddPolicy("auth-policy", context =>
                {
                    var ipAddress =
                        context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = isTesting ? 1000 : 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });

                // for every controller except auth: user can reqest 100 per min per their ip
                options.AddPolicy("general-policy", context =>
                {
                    var userId =
                        context.User.FindFirst(ClaimConstants.UserId)?.Value;

                    var partitionKey =
                        !string.IsNullOrWhiteSpace(userId)
                            ? $"user:{userId}"
                            : $"ip:{context.Connection.RemoteIpAddress}";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = isTesting ? 1000 : 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });

                options.AddPolicy("swagger-login-policy", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = isTesting ? 1000 : 3,
                            Window = TimeSpan.FromMinutes(15),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    var policyName = context.HttpContext.GetEndpoint()?.Metadata
                        .GetMetadata<EnableRateLimitingAttribute>()?.PolicyName;

                    string message = policyName switch
                    {
                        "check-email" => "Too many requests. Please try again later.",
                        "verify-otp" => "Too many OTP verification attempts. Please wait a few minutes and try again.",
                        "create-password" => "Too many password creation attempts. Please wait a few minutes and try again.",
                        "otp" => "Too many OTP requests. Please try again after 10 minutes.",
                        "login" => "Too many login attempts. Please try again after 5 minutes.",
                        "refresh" => "Too many refresh token attempts. Please try again after 10 minutes.",
                        "change-password" => "Too many password change attempts. Please try again tomorrow.",
                        "swagger-login-policy" => "Too many login attempts. Please try again after 15 minutes.",
                        _ => "Too many requests. Please slow down."
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        statusCode = 429,
                        message
                    }, cancellationToken);
                };
            });

            return services;
        }
    }
}
