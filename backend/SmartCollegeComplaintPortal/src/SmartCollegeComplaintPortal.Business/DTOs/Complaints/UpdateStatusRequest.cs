using SmartCollegeComplaintPortal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class UpdateStatusRequest
    {
        [Required]
        public ComplaintStatus NewStatus { get; set; }
    }
}
