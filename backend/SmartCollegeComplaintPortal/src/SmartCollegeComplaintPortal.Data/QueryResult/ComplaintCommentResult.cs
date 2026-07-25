namespace SmartCollegeComplaintPortal.Data.QueryResult
{
    public class ComplaintCommentResult
    {
        public int ComplaintCommentId { get; set; }
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserRole { get; set; }
    }
}
