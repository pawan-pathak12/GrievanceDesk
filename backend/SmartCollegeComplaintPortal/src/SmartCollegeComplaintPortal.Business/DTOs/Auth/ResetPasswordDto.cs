using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification token is required.")]
        public string VerificationToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
