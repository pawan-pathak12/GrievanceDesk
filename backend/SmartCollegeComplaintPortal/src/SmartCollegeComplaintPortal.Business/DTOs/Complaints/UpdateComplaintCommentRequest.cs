using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class UpdateComplaintCommentRequest
    {
        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}
