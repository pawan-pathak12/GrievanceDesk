using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Domain.Entities.Complaints
{
    public class Complaint
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ComplaintCategory Category { get; set; } = ComplaintCategory.General;
        public string Location { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public bool IsDuplicate { get; set; } = false;
        public bool IsForwarded { get; set; } = false;
        public bool IsEdited { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
