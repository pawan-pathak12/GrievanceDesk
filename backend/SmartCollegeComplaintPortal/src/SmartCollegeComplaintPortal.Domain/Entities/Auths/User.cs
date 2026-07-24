using DotNetApiStarterKit.Domain.Enums;

namespace DotNetApiStarterKit.Domain.Entities.Auths
{
    public class User
    {
        public int UserId { get; set; }
        //   public int? StaffId { get; set; }
        //       public Staff? Staff { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        //       public bool IsTemporaryPassword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
