namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class ComplaintDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsDuplicate { get; set; }
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // History timeline
        public List<ComplaintUpdateResponse> Updates { get; set; } = new();
    }
}
