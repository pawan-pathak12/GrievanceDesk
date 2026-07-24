using DotNetApiStarterKit.Data.Database;
using DotNetApiStarterKit.Data.Interfaces;
using DotNetApiStarterKit.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetApiStarterKit.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<SystemDbContext>();
            services.AddDbContext<SystemEfCoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("StudentCourseManagement.Data")
                    );

            });
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();

            return services;
        }
    }
}
