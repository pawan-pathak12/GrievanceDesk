using DotNetApiStarterKit.Domain.Entities.Auths;
using Microsoft.EntityFrameworkCore;

namespace DotNetApiStarterKit.Data.Database
{
    public class SystemEfCoreDbContext : DbContext
    {
        public SystemEfCoreDbContext(DbContextOptions<SystemEfCoreDbContext> options) : base(options)
        {

        }

        public DbSet<FileMetadata> FileMetadatas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Otp> Otps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
