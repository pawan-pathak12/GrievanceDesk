using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCollegeComplaintPortal.Data.Database;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Data.Repositories;

namespace SmartCollegeComplaintPortal.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<SystemDbContext>();
            services.AddDbContext<SystemEfCoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SmartCollegeComplaintPortal.Data")
                    );

            });
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IComplaintCommentRepository, ComplaintCommentRepository>();
            services.AddScoped<INoticeRepository, NoticeRepository>();

            return services;
        }
    }
}
