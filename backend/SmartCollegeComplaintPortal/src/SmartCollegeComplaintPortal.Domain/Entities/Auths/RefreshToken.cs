namespace SmartCollegeComplaintPortal.Domain.Entities.Auths
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked { get; set; }

    }
}
