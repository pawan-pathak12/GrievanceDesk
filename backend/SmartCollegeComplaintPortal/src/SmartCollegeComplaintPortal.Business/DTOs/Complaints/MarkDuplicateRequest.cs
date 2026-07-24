namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class MarkDuplicateRequest
    {
        public bool IsDuplicate { get; set; } = true;
        public string? NoteText { get; set; }
    }
}
