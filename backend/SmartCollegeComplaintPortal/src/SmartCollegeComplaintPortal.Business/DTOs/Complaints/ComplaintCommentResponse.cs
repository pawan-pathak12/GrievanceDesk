namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class ComplaintCommentResponse
    {
        public int ComplaintCommentId { get; set; }
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}
