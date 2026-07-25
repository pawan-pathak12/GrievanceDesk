using System.ComponentModel.DataAnnotations;
using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Business.DTOs.Notices
{
    public class UpdateNoticeDto
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(10000)]
        public string Content { get; set; } = string.Empty;

        public NoticePriority Priority { get; set; } = NoticePriority.Normal;
    }
}
