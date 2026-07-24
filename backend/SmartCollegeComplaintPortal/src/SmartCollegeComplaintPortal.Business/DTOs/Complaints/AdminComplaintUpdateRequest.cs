using SmartCollegeComplaintPortal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class AdminComplaintUpdateRequest
    {
        [EnumDataType(typeof(ComplaintStatus), ErrorMessage = "Invalid complaint status.")]
        public ComplaintStatus Status { get; set; }

        [StringLength(2000, ErrorMessage = "Resolution note cannot exceed 2000 characters.")]
        public string? Note { get; set; }

        public bool IsDuplicate { get; set; }

        public bool IsIrrelevant { get; set; }

        [StringLength(2000, ErrorMessage = "Irrelevant reason cannot exceed 2000 characters.")]
        public string? IrrelevantReason { get; set; }
    }
}
