using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class MarkIrrelevantRequest
    {
        [Required(ErrorMessage = "An irrelevant reason is required.")]
        [StringLength(2000, MinimumLength = 3, ErrorMessage = "Irrelevant reason must be between 3 and 2000 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}
