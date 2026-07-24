using SmartCollegeComplaintPortal.API.Middlewares;

namespace SmartCollegeComplaintPortal.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            return app;

        }
        public static IApplicationBuilder UseSwaggerGate(this IApplicationBuilder app)
        {
            app.UseMiddleware<SwaggerAuthMiddleware>();
            return app;
        }
    }
}
