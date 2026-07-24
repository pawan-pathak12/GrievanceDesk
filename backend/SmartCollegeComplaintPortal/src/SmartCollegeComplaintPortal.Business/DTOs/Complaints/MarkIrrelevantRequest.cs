using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class MarkIrrelevantRequest
    {
        [StringLength(2000)]
        public string? Reason { get; set; }
    }
}
