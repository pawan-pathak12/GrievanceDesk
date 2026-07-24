using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Users
{
    public class CreateUserByAdminRequest
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
