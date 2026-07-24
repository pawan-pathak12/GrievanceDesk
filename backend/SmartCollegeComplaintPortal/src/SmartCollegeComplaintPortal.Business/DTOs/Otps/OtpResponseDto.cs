using System.ComponentModel.DataAnnotations;

namespace DotNetApiStarterKit.Business.DTOs.Otps
{
    public class OtpResponseDto
    {
        public int OtpId { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
        
        public DateTime ExpiresAt { get; set; }
    }
}
