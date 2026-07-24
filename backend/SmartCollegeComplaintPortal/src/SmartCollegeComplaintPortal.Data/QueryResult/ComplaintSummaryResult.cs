namespace SmartCollegeComplaintPortal.Data.QueryResult
{
    public class ComplaintSummaryResult
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Category { get; set; }
        public int Status { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
