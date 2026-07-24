using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Domain.Entities.Complaints
{
    public class ComplaintUpdate
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }
        public int UpdatedByUserId { get; set; }
        public ComplaintUpdateType UpdateType { get; set; }
        public ComplaintStatus? OldStatus { get; set; }
        public ComplaintStatus? NewStatus { get; set; }
        public string? NoteText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
