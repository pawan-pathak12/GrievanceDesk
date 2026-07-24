using SmartCollegeComplaintPortal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Users
{
    public class UserResponseDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid user role.")]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}