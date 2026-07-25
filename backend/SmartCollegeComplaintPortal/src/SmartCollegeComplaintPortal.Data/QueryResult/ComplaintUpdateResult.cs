namespace SmartCollegeComplaintPortal.Data.QueryResult
{
    public class ComplaintUpdateResult
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int UpdatedByUserId { get; set; }
        public int UpdateType { get; set; }
        public int? OldStatus { get; set; }
        public int? NewStatus { get; set; }
        public string? NoteText { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
