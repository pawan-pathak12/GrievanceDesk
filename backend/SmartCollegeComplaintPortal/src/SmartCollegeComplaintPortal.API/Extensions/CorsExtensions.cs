namespace SmartCollegeComplaintPortal.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(
          this IServiceCollection services,
          IConfiguration configuration,
          IHostEnvironment environment)
        {
            var allowedOrigins = configuration
                .GetSection("AllowedOrigins")
                .Get<string[]>() ?? [];

            if (environment.IsDevelopment())
            {
                var localOrigins = new[]
                {
                    "http://localhost:5173",
                    "http://127.0.0.1:5173",
                    "http://localhost:3000",
                    "http://127.0.0.1:3000",
                    "https://localhost:3000",
                    "https://127.0.0.1:3000"
                };

                allowedOrigins = allowedOrigins
                    .Concat(localOrigins)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            return services;
        }
    }
}
