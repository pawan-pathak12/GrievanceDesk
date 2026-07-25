namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class ComplaintUpdateResponse
    {
        public int Id { get; set; }
        public string UpdateType { get; set; } = string.Empty;
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? NoteText { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
