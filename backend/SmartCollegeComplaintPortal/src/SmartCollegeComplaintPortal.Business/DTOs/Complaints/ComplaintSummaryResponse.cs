namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class ComplaintSummaryResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsDuplicate { get; set; }
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string SubmitterName { get; set; } = string.Empty;
    }
}
