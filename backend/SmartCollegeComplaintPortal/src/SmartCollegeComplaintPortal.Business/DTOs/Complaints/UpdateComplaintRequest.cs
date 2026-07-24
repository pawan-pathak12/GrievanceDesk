using SmartCollegeComplaintPortal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartCollegeComplaintPortal.Business.DTOs.Complaints
{
    public class UpdateComplaintRequest
    {
        [Required, StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(5000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ComplaintCategory Category { get; set; }

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;
    }
}
