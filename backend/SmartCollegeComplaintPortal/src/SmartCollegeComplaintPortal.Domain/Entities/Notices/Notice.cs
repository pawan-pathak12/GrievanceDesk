using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Domain.Entities.Notices
{
    public class Notice
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NoticePriority Priority { get; set; } = NoticePriority.Normal;
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
