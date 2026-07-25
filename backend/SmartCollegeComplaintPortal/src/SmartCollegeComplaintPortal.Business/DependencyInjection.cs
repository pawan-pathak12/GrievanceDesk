using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;
using SmartCollegeComplaintPortal.Business.Interfaces.Notices;
using SmartCollegeComplaintPortal.Business.Services.Auths;
using SmartCollegeComplaintPortal.Business.Services.Complaints;
using SmartCollegeComplaintPortal.Business.Services.Notices;
using SmartCollegeComplaintPortal.Data;
using SmartCollegeComplaintPortal.Data.Database;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;

namespace SmartCollegeComplaintPortal.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));
            services.AddDataLayer(configuration);
            services.AddHttpContextAccessor();


            services.AddHealthChecks()
       .AddDbContextCheck<SystemEfCoreDbContext>("database");

            #region Register Services

            services.AddScoped<IFileStorageService,
                AzureBlobStorageService>();


            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<INoticeService, NoticeService>();

            #endregion

            return services;
        }

        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SystemEfCoreDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SystemEfCoreDbContext>>();

            var pendingMigrations = db.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count > 0)
            {
                logger.LogInformation("Applying {Count} pending migration(s): {Migrations}",
                    pendingMigrations.Count, string.Join(", ", pendingMigrations));

                db.Database.Migrate();

                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations. Database is up to date.");
            }
        }

    }
}
