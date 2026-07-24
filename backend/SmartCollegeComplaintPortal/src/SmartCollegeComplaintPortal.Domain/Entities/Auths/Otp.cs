namespace SmartCollegeComplaintPortal.Domain.Entities.Auths
{
    public class Otp
    {
        public int OtpId { get; set; }
        public string? Email { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }

    }

}
