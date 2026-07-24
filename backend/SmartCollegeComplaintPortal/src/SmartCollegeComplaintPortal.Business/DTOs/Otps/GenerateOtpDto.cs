using System.ComponentModel.DataAnnotations;

namespace DotNetApiStarterKit.Business.DTOs.Otps
{
    public class GenerateOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}