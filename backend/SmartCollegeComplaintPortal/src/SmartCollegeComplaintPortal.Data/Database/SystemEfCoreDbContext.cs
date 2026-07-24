using Microsoft.EntityFrameworkCore;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;
using SmartCollegeComplaintPortal.Domain.Entities.Members;
using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Data.Database
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
        public DbSet<Member> Members { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintUpdate> ComplaintUpdates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ──── Member ────
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemberId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Role).HasDefaultValue(UserRole.User);
                entity.Property(e => e.IsRegistered).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.Email).IsUnique();
            });

            // ──── User (additions) ────
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50).HasDefaultValue("");
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // ──── Complaint ────
            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Category).HasDefaultValue(ComplaintCategory.General);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).HasDefaultValue(ComplaintStatus.Pending);
                entity.Property(e => e.IsDuplicate).HasDefaultValue(false);
                entity.Property(e => e.IsEdited).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Category);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ──── ComplaintUpdate ────
            modelBuilder.Entity<ComplaintUpdate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UpdateType).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.ComplaintId);

                entity.HasOne<Complaint>()
                    .WithMany()
                    .HasForeignKey(e => e.ComplaintId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
