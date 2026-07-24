using System.ComponentModel.DataAnnotations;

namespace DotNetApiStarterKit.Business.DTOs.Otps
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 characters.")]
        public string Code { get; set; } = string.Empty;
    }
}
